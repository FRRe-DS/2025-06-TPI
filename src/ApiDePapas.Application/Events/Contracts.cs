namespace ApiDePapas.Application.Events;

public record OrderCreated(int OrderId, int UserId, List<OrderItem> Items, AddressDto DeliveryAddress, DateTime OccurredAtUtc, Guid EventId);
public record OrderItem(int ProductId, int Quantity);
public record AddressDto(string Street, string City, string State, string PostalCode, string Country);

public record StockReserved(int OrderId, int ReservationId, DateTime ExpiresAtUtc, Guid EventId);
public record StockReservationFailed(int OrderId, string Reason, Guid EventId);

public record ShippingCreated(int OrderId, int ShippingId, string TransportType, DateTime EstimatedDeliveryAtUtc, Guid EventId);

public record OrderCancelled(int OrderId, string Reason, Guid EventId);
