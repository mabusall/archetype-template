namespace Archetype.Infrastructure.Tasks;

public static class TaskManager
{
    public static void RegisterHangfireJob<THandler>(IRecurringJobManager recurringJobManager,
                                          string jobKey,
                                          string cron,
                                          Expression<Func<THandler, Task>> expression,
                                          bool immediateRun = false)
        where THandler : class
    {
        recurringJobManager.RemoveIfExists(jobKey);
        recurringJobManager.AddOrUpdate(jobKey, expression, cron);

        if (immediateRun) RecurringJob.TriggerJob(jobKey);
    }

    /// <summary>
    /// Registers a recurring Hangfire job dynamically using reflection.
    /// Supports jobs with or without CancellationToken parameter.
    /// </summary>
    public static void RegisterRecurringJob(string jobId, Type jobType, string cronExpression)
    {
        // Create the method call dynamically
        MethodInfo methodInfo = typeof(TaskManager).GetMethod(nameof(RegisterRecurringJobGeneric),
            BindingFlags.Static | BindingFlags.NonPublic)!;

        MethodInfo genericMethod = methodInfo.MakeGenericMethod(jobType);

        genericMethod.Invoke(null, [jobId, cronExpression]);
    }

    private static void RegisterRecurringJobGeneric<T>(string jobId, string cronExpression)
        where T : IHangfireJob
    {
        // Remove existing job with the same ID to avoid duplicates
        RecurringJob.RemoveIfExists(jobId);

        // Simple registration - Hangfire will inject CancellationToken automatically
        RecurringJob.AddOrUpdate<T>(jobId,
                                    job => job.ExecuteAsync(CancellationToken.None),
                                    cronExpression);
    }
}
