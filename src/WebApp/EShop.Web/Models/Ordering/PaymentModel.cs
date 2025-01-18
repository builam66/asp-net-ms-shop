namespace EShop.Web.Models.Ordering
{
    public record PaymentModel(
        string CardName,
        string CardNumber,
        string Expiration,
        string Cvv,
        int PaymentMethod);
}
