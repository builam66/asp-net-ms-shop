using BuildingBlocks.CQRS;
using BuildingBlocks.Common;
using FluentValidation;

namespace Catalog.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    List<string> Categories,
    string Description,
    string ImageUri,
    decimal Price,
    int Stock = 0
) : ICommand<Result<CreateProductResult>>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(x => x.Categories)
            .NotEmpty().WithMessage("At least one category is required")
            .Must(categories => categories.All(c => !string.IsNullOrWhiteSpace(c)))
            .WithMessage("All categories must be valid");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.ImageUri)
            .NotEmpty().WithMessage("Product image is required")
            .Must(BeAValidUrl).WithMessage("Image URI must be a valid URL");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThan(1000000).WithMessage("Price must be less than 1,000,000");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}