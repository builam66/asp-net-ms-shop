namespace Ordering.Infrastructure.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<Customer> Customers =>
        [
            Customer.Create(CustomerId.Of(new Guid("58c49479-ec65-4de2-86e7-033c546291aa")), "usera", "usera@gmail.com"),
            Customer.Create(CustomerId.Of(new Guid("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d")), "userb", "userb@gmail.com")
        ];

        public static IEnumerable<Product> Products =>
        [
            Product.Create(ProductId.Of(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61")), "IPhone X", 500),
            Product.Create(ProductId.Of(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914")), "Samsung 10", 400),
            Product.Create(ProductId.Of(new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8")), "Huawei Plus", 650),
            Product.Create(ProductId.Of(new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27")), "Xiaomi Mi", 450)
        ];

        public static IEnumerable<Order> OrdersWithOrderItems
        {
            get
            {
                var address1 = Address.Of("usera", "test", "usera@gmail.com", "Pham Van Dong", "Vietnam", "HCM", "12345");
                var address2 = Address.Of("userb", "test", "userb@gmail.com", "QL13", "Vietnam", "HCM", "56789");

                var payment1 = Payment.Of("usera", "0000000000000000", "01/30", "123", 1);
                var payment2 = Payment.Of("userb", "1111111111111111", "01/30", "456", 2);

                var order1 = Order.Create(
                                OrderId.Of(Guid.NewGuid()),
                                CustomerId.Of(new Guid("58c49479-ec65-4de2-86e7-033c546291aa")),
                                orderName: "ORD_1",
                                shippingAddress: address1,
                                billingAddress: address1,
                                payment1);
                order1.Add(ProductId.Of(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61")), 2, 500);
                order1.Add(ProductId.Of(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914")), 1, 400);

                var order2 = Order.Create(
                                OrderId.Of(Guid.NewGuid()),
                                CustomerId.Of(new Guid("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d")),
                                orderName: "ORD_2",
                                shippingAddress: address2,
                                billingAddress: address2,
                                payment2);
                order2.Add(ProductId.Of(new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8")), 1, 650);
                order2.Add(ProductId.Of(new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27")), 2, 450);

                return [order1, order2];
            }
        }
    }
}
