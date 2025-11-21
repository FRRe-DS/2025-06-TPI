namespace ApiDePapas.Domain.ValueObjects;

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