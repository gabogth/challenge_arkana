using FluentValidation;

namespace transaction_application.Commands.CreateTransaction
{
    public class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionValidator()
        {
            RuleFor(x => x.SourceAccountId)
                .NotEmpty().WithMessage("AccountExternalIdDebit is required.");

            RuleFor(x => x.TargetAccountId)
                .NotEmpty().WithMessage("AccountExternalIdCredit is required.");

            RuleFor(x => x.SourceAccountId)
                .NotEqual(x => x.TargetAccountId).WithMessage("SourceAccountId and TargetAccountId cannot be the same.");

            RuleFor(x => x.TransferTypeId)
                .InclusiveBetween(0, 1).WithMessage("TransferTypeId must be 0 or 1.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Value must be greater than zero.");
        }
    }
}
