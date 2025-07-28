using BuildingBlocks.CQRS;
using BuildingBlocks.Common;
using BuildingBlocks.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductCommandHandler> logger)
    : ICommandHandler<CreateProductCommand, Result<CreateProductResult>>
{
    public async Task<Result<CreateProductResult>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product with name: {ProductName}", command.Name);

        // Check if product name is unique
        var isNameUnique = await productRepository.IsProductNameUniqueAsync(command.Name, cancellationToken: cancellationToken);
        if (!isNameUnique)
        {
            logger.LogWarning("Product creation failed - name already exists: {ProductName}", command.Name);
            return Result.Failure<CreateProductResult>("A product with this name already exists");
        }

        try
        {
            // Create product domain entity
            var product = new Product(
                command.Name,
                command.Categories,
                command.Description,
                command.ImageUri,
                command.Price,
                command.Stock
            );

            // Save to repository
            await productRepository.AddAsync(product, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

            return Result.Success(new CreateProductResult(product.Id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product with name: {ProductName}", command.Name);
            return Result.Failure<CreateProductResult>("An error occurred while creating the product");
        }
    }
}