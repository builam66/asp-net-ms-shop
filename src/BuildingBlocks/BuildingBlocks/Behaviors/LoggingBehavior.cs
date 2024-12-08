using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
        (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] {Request} - {Response} - RequestData: {RequestData}",
                typeof(TRequest).Name, typeof(TResponse).Name, request);

            var timer = new Stopwatch();
            timer.Start();

            var response = await next();

            timer.Stop();
            var time = timer.Elapsed;
            if (time.Seconds > 3)
            {
                logger.LogWarning("[PERFORMANCE] {Request} took {Time} seconds",
                    typeof(TRequest).Name, time.Seconds);
            }

            logger.LogInformation("[END] {Request} - {Response} - ResponseData: {ResponseData}",
                typeof(TRequest).Name, typeof(TResponse).Name, response);

            return response;
        }
    }
}
