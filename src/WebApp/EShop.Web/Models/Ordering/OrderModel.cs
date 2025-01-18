namespace EShop.Web.Models.Ordering
{
    public record OrderModel(
        Guid Id,
        Guid CustomerId,
        string OrderName,
        AddressModel ShippingAddress,
        AddressModel BillingAddress,
        PaymentModel Payment,
        OrderStatus Status,
        List<OrderItemModel> OrderItems);

    public record OrderItemModel(Guid OrderId, Guid ProductId, int Quantity, decimal Price);

    public enum OrderStatus
    {
        Draft = 1,
        Pending = 2,
        Completed = 3,
        Cancelled = 4,
    }

    public record GetOrdersResponse(PaginatedModel<OrderModel> Orders);

    public record GetOrdersByNameResponse(IEnumerable<OrderModel> Orders);

    public record GetOrdersByCustomerResponse(IEnumerable<OrderModel> Orders);
}
