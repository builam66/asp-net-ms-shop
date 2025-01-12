namespace Ordering.Application.DTOs
{
    public record AddressDto(
        string FirstName,
        string LastName,
        string Email,
        string AddressLine,
        string Country,
        string City,
        string ZipCode);
}
