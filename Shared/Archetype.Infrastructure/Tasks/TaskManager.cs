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

        if (immediateRun) Hangfire.RecurringJob.TriggerJob(jobKey);
    }

    /// <summary>
    /// Enqueues a fire-and-forget Hangfire job using a strongly typed expression.
    /// </summary>
    public static string EnqueueJob<THandler>(Expression<Func<THandler, Task>> expression)
        where THandler : class
        => BackgroundJob.Enqueue(expression);

    /// <summary>
    /// Executes a one-time background job (fire-and-forget) using a strongly typed expression.
    /// </summary>
    public static string RunOnceJob<THandler>(Expression<Func<THandler, Task>> expression)
        where THandler : class
        => EnqueueJob(expression);

    /// <summary>
    /// Enqueues a fire-and-forget Hangfire job dynamically using reflection.
    /// The job type must implement <see cref="IHangfireJob"/>.
    /// </summary>
    public static string EnqueueJob(Type jobType)
    {
        MethodInfo methodInfo = typeof(TaskManager).GetMethod(nameof(EnqueueJobGeneric),
            BindingFlags.Static | BindingFlags.NonPublic)!;

        MethodInfo genericMethod = methodInfo.MakeGenericMethod(jobType);

        return (string)genericMethod.Invoke(null, null)!;
    }

    /// <summary>
    /// Executes a one-time background job (fire-and-forget) dynamically using reflection.
    /// </summary>
    public static string RunOnceJob(Type jobType)
        => EnqueueJob(jobType);

    /// <summary>
    /// Registers a recurring Hangfire job dynamically using reflection.
    /// Supports jobs with or without CancellationToken parameter.
    /// </summary>
    public static void RecurringJob(string jobId, Type jobType, string cronExpression)
    {
        // Create the method call dynamically
        MethodInfo methodInfo = typeof(TaskManager).GetMethod(nameof(RecurringJobGeneric),
            BindingFlags.Static | BindingFlags.NonPublic)!;

        MethodInfo genericMethod = methodInfo.MakeGenericMethod(jobType);

        genericMethod.Invoke(null, [jobId, cronExpression]);
    }

    private static string EnqueueJobGeneric<T>()
        where T : IHangfireJob
        => BackgroundJob.Enqueue<T>(job => job.ExecuteAsync(CancellationToken.None));

    private static void RecurringJobGeneric<T>(string jobId, string cronExpression)
        where T : IHangfireJob
    {
        // Remove existing job with the same ID to avoid duplicates
        Hangfire.RecurringJob.RemoveIfExists(jobId);

        // Simple registration - Hangfire will inject CancellationToken automatically
        Hangfire.RecurringJob.AddOrUpdate<T>(jobId,
                                    job => job.ExecuteAsync(CancellationToken.None),
                                    cronExpression);
    }
}
