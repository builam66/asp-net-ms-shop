namespace BuildingBlocks.Exceptions
{
    public class InternalServerException : Exception
    {
        public string? Details { get; }

        public InternalServerException(string message) : base(message)
        {
        }

        public InternalServerException(string message, string details) : base(message)
        {
            Details = details;
        }
    }
}
