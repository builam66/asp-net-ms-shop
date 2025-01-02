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

        protected Address()
        {
        }

        private Address(string firstName, string lastName, string email, string addressLine, string country, string city, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AddressLine = addressLine;
            Country = country;
            City = city;
            ZipCode = zipCode;
        }

        public static Address Of(string firstName, string lastName, string email, string addressLine, string country, string city, string zipCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine);

            return new Address(firstName, lastName, email, addressLine, country, city, zipCode);
        }
    }
}
