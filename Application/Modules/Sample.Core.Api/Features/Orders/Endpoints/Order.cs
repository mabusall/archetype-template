namespace Sample.Core.Api.Features.Orders.Endpoints;

public static class OrderEndpoints
{
    private const string OrderManagementsTag = "Order Managements";
    private const string TenRequestsPerMinutePolicy = "10RequestsPerMinute";

    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        // Create API version set
        ApiVersionSet versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        // Map group with versioning (v1/order)
        var group = app
            .MapGroup("v{version:apiVersion}/order")
            .WithApiVersionSet(versionSet);

        group.MapPost("/", async ([FromBody] CreateOrderCommand command,
                                  IMediator sender,
                                  CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(command, cancellationToken);

            return Results.Ok(response);
        })
        .WithName("CreateOrder")
        .WithSummary("Create a new order")
        .WithDescription("This endpoint allows you to create a new order.")
        .WithTags(OrderManagementsTag)
        .Produces<CreateOrderResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireApiKey()
        //.RequireAuthorization()
        .RequireRateLimiting(TenRequestsPerMinutePolicy);

        group.MapGet("/", async ([AsParameters] GetListOrderQuery query,
                                 IMediator sender,
                                 CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(query, cancellationToken);

            return Results.Ok(response);
        })
        .WithName("GetListOrders")
        .WithSummary("Get orders list")
        .WithDescription("This endpoint allows you to get orders list.")
        .WithTags(OrderManagementsTag)
        .Produces<CreateOrderResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireApiKey()
        .CacheOutput("Default1Minute")
        //.CacheOutput(policy => policy.Expire(TimeSpan.FromMinutes(5)))
        //.RequireAuthorization()
        .RequireRateLimiting(TenRequestsPerMinutePolicy);

        group.MapDelete("/{orderId:int}", async ([FromRoute] int orderId,
                                                 IMediator sender,
                                                 CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteOrderCommand(orderId),
                                                           cancellationToken);

            return Results.Ok(response);
        })
        .WithName("DeleteOrder")
        .WithSummary("Delete an order")
        .WithDescription("This endpoint allows you to delete an order.")
        .WithTags(OrderManagementsTag)
        .Produces<DeleteOrderResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireApiKey()
        //.RequireAuthorization()
        .RequireRateLimiting(TenRequestsPerMinutePolicy);
    }
}