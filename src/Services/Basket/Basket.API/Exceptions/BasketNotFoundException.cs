namespace Basket.API.Exceptions
{
    public class BasketNotFoundException : NotFoundException
    {
        public BasketNotFoundException(string username) : base("Basket", username)
        {
            
        }
    }
}
