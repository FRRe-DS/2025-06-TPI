using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;              // <-- para ToSnakeCase
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Services
{
    public class ShippingService : IShippingService
    {
        private readonly IShippingStore _store;
        public ShippingService(IShippingStore store) => _store = store;

        public ShippingListResponse List(
            int? userId,
            ShippingStatus? status,
            DateOnly? fromDate,
            DateOnly? toDate,
            int page,
            int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 1;
            if (limit > 100) limit = 100;

            var q = _store.GetAll().AsQueryable();

            if (userId.HasValue) q = q.Where(s => s.user_id == userId.Value);
            if (status.HasValue) q = q.Where(s => s.status == status.Value);

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                q = q.Where(s => s.created_at >= from);
            }
            if (toDate.HasValue)
            {
                var to = toDate.Value.ToDateTime(TimeOnly.MaxValue);
                q = q.Where(s => s.created_at <= to);
            }

            q = q.OrderByDescending(s => s.created_at);

            var totalItems = q.Count();
            var pageItems  = q.Skip((page - 1) * limit).Take(limit).ToList();

            // helper local: PascalCase -> snake_case
            static string ToSnakeCase(string s) =>
                Regex.Replace(s, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();

            // MAP: ShippingDetail -> ShipmentSummary (record posicional)
            var summaries = pageItems.Select(s =>
                new ShipmentSummary(
                    s.shipping_id,
                    s.order_id,
                    s.user_id,
                    s.products,
                    ToSnakeCase(s.status.ToString()),         // enum -> "in_transit"
                    ToSnakeCase(s.transport_type.ToString()), // enum -> "air" / "sea" / etc.
                    s.estimated_delivery_at,
                    s.created_at
                )
            ).ToList();

            // ✅ Constructor del record, usando argumentos con nombre
            var pagination = new PaginationData(
                current_page:   page,
                total_pages:    (int)Math.Ceiling(totalItems / (double)limit),
                total_items:    totalItems,
                items_per_page: limit
            );

            return new ShippingListResponse(summaries, pagination);
        }
    }
}
