using MassTransit;
using ApiDePapas.Application.Events;
using ApiDePapas.Application.Interfaces;

namespace ApiDePapas.Consumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelled>
{
    private readonly IShippingStore _store;

    public OrderCancelledConsumer(IShippingStore store)
    {
        _store = store;
    }

    public Task Consume(ConsumeContext<OrderCancelled> ctx)
    {
        return Task.CompletedTask;
    }
}
