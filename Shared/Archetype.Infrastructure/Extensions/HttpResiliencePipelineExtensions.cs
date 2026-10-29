namespace Archetype.Infrastructure.Extensions;

public static class HttpResiliencePipelineExtensions
{
    extension(ResiliencePipelineBuilder<HttpResponseMessage> pipeline)
    {
        public void AddStandardCircuitBreaker(Microsoft.Extensions.Logging.ILogger logger, string apiName)
        {
            pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(15),
                OnOpened = args =>
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                        logger.LogWarning("[{ApiName}] Circuit OPENED for {BreakDuration}.", apiName, args.BreakDuration);
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation("[{ApiName}] Circuit HALF-OPEN.", apiName);
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation("[{ApiName}] Circuit CLOSED.", apiName);
                    return ValueTask.CompletedTask;
                }
            });
        }
    }
}