namespace Sample.Core.Api.Features.Orders;

public record ChangeStatusOrderCommand
    : ICommand<ChangeStatusOrderResult>
{
    public Guid UserId { get; private set; }

    public bool IsActive { get; set; }

    internal void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}

public record ChangeStatusOrderResult(bool IsSuccess);