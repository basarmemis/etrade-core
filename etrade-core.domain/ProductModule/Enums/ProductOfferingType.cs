namespace etrade_core.domain.ProductModule.Enums
{
    [Flags]
    public enum ProductOfferingTypes
    {
        None = 0,
        Rental = 1,
        Purchsase = 2,
        Subscription = 4,
        Service = 8,
        Membership = 16,
        Other = 32
    }
}