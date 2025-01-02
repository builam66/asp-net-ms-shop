namespace Ordering.Domain.ValueObjects
{
    public record OrderId
    {
        public Guid Value { get; }

        private OrderId(Guid value) => Value = value;

        public static OrderId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value == Guid.Empty)
            {
                throw new DomainException("OrderId cannot null");
            }

            return new OrderId(value);
        }
    }

    public record CustomerId
    { 
        public Guid Value { get; }

        private CustomerId(Guid value) => Value = value;

        public static CustomerId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value == Guid.Empty)
            {
                throw new DomainException("CustomerId cannot null");
            }

            return new CustomerId(value);
        }
    }

    public record OrderItemId
    {
        public Guid Value { get; }

        private OrderItemId(Guid value) => Value = value;

        public static OrderItemId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value == Guid.Empty)
            {
                throw new DomainException("OrderItemId cannot null");
            }

            return new OrderItemId(value);
        }
    }

    public record ProductId
    {
        public Guid Value { get; }

        private ProductId(Guid value) => Value = value;

        public static ProductId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value == Guid.Empty)
            {
                throw new DomainException("ProductId cannot null");
            }

            return new ProductId(value);
        }
    }
}
