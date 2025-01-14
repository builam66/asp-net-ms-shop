using Ordering.Application.Order.Commands.CancelOrder;

namespace Ordering.API.Endpoints
{
    //public record CancelOrderRequest(Guid Id);

    public record CancelOrderResponse(bool IsSuccess);

    public class CancelOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/orders/{id:guid}/cancel", async (Guid Id, ISender sender) =>
            {
                var result = await sender.Send(new CancelOrderCommand(Id));

                var response = result.Adapt<CancelOrderResponse>();

                return Results.Ok(response);
            })
            .WithName("CancelOrder")
            .Produces<CancelOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Cancel Order")
            .WithDescription("Cancel Order");
        }
    }
}
