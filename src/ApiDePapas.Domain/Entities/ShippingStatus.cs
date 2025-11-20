namespace ApiDePapas.Domain.Entities
{
    public enum ShippingStatus
    {
        Created,
        Reserved,
        InTransit,
        Delivered,
        Canceled,
        InDistribution,
        Arrived
    }
}