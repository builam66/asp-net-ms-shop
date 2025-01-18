namespace EShop.Web.Models.Ordering
{
    public record AddressModel(
        string FirstName,
        string LastName,
        string Email,
        string AddressLine,
        string Country,
        string City,
        string ZipCode);
}
