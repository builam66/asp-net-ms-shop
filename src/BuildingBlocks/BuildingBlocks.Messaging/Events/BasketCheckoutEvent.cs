namespace BuildingBlocks.Messaging.Events
{
    public record BasketCheckoutEvent : IntegrationEvent
    {
        public string Username { get; set; } = default!;

        public Guid CustomerId { get; set; } = default!;

        public decimal TotalPrice { get; set; } = default!;

        // Shipping and Billing Address
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string AddressLine { get; set; } = default!;

        public string Country { get; set; } = default!;

        public string City { get; set; } = default!;

        public string ZipCode { get; set; } = default!;

        // Payment
        public string CardName { get; set; } = default!;

        public string CardNumber { get; set; } = default!;

        public string Expiration { get; set; } = default!;

        public string CVV { get; set; } = default!;

        public int PaymentMethod { get; set; } = default!;
    }
}
