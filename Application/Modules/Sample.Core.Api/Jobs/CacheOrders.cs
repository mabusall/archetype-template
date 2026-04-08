namespace Sample.Core.Api.Jobs;

class CacheOrdersJob(ISampleDbUnitOfWork uow,
                     IDistributedCache distributedCache)
   : IHangfireJob
{
    [DisableConcurrentExecution(timeoutInSeconds: 5 * 60)] // 5 minutes max
    public async Task ExecuteAsync()
    {
        // Fetch all orders with cancellation support
        List<Order> allOrders = await uow
            .OrdersRepository
            .GetListAsync(a => true, CancellationToken.None);

        // Serialize orders
        string allOrdersJson = allOrders.Serialize()!;

        // Cache with expiration and cancellation support
        DistributedCacheEntryOptions cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        await distributedCache.SetStringAsync(key: "orders",
                                              value: allOrdersJson,
                                              options: cacheOptions,
                                              token: CancellationToken.None);
    }
}