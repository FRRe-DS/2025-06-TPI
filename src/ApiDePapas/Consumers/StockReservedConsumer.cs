using MassTransit;
using ApiDePapas.Application.Events;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Consumers;

public class StockReservedConsumer : IConsumer<StockReserved>
{
    private readonly IShippingService _shipping;

    public StockReservedConsumer(IShippingService shipping)
    {
        _shipping = shipping;
    }

    public async Task Consume(ConsumeContext<StockReserved> ctx)
    {
        var ev = ctx.Message;

        var request = new CreateShippingRequest
        {
            order_id = ev.OrderId,
            user_id = 0, // completar si el evento lo trae o consultar a Compras
            delivery_address = new AddressWriteDto
            {
                street = "From Reservation",
                number = 0,
                postal_code = "H3500ABC",
                locality_name = "Resistencia"
            },
            transport_type = "road",
            products = new List<ProductRequestDto>()
        };

        var created = await _shipping.CreateNewShipping(request);

        await ctx.Publish(new ShippingCreated(
            OrderId: request.order_id,
            ShippingId: created.shipping_id,
            TransportType: request.transport_type,
            EstimatedDeliveryAtUtc: created.estimated_delivery_at,
            EventId: Guid.NewGuid()
        ));
    }
}
