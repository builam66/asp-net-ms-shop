namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name, List<string> Categories, string Description, string ImageUri, decimal Price)
        : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Categories).NotEmpty().WithMessage("Categories is required");
            RuleFor(x => x.ImageUri).NotEmpty().WithMessage("Image is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }

    internal class CreateProductCommandHandler
        (IDocumentSession session, ILogger<CreateProductCommandHandler> logger)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("CreateProductCommandHandler: {@Command}", command);

            var product = new Product
            {
                Name = command.Name,
                Categories = command.Categories,
                Description = command.Description,
                ImageUri = command.ImageUri,
                Price = command.Price,
            };

            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);
            
            return new CreateProductResult(product.Id);
        }
    }
}
