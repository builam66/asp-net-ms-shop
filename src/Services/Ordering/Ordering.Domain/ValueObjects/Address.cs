namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
        public string FirstName { get; } = default!;

        public string LastName { get; } = default!;

        public string? Email { get; } = default!;

        public string AddressLine {  get; } = default!;

        public string Country { get; } = default!;

        public string City { get; } = default!;

        public string ZipCode { get; } = default!;
    }
}
