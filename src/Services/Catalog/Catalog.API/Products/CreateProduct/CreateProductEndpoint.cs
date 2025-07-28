using Catalog.Application.Products.Commands.CreateProduct;
using Mapster;
using MediatR;

namespace Catalog.API.Products.CreateProduct;

public record CreateProductRequest(string Name, List<string> Categories, string Description, string ImageUri, decimal Price, int Stock = 0);

public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products",
            async (CreateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateProductCommand>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { Error = result.Error });
            }

            var response = result.Value.Adapt<CreateProductResponse>();

            return Results.Created($"/products/{response.Id}", response);
        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product")
        .WithDescription("Create Product");
    }
}
