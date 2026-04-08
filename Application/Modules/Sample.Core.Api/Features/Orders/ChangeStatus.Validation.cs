//namespace Tasheer.Core.Api.Features.Orders;

//internal class ChangeStatusOrderCommandValidator : AbstractValidator<ChangeStatusOrderCommand>
//{
//    public ChangeStatusOrderCommandValidator(IFlixHubDbUnitOfWork uow)
//    {
//        RuleFor(r => r.UserId)
//            .NotEmpty()
//            .WithMessage(ErrorMessageResources.NotEmpty)
//            .DependentRules(() =>
//            {
//                RuleFor(x => x.UserId)
//                    .MustAsync(async (command, userId, _, cancellationToken) =>
//                        await uow
//                            .OrdersRepository
//                            .AnyAsync(su => su.Uuid == userId, cancellationToken))
//                    .WithMessage(ErrorMessageResources.UserManagement_InvalidAccount);
//            });
//    }
//}