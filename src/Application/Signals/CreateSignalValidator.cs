using FluentValidation;

namespace ProyectoForex.Application.Signals;
public sealed class CreateSignalValidator : AbstractValidator<CreateSignalCommand>
{
   public CreateSignalValidator()
    {
        RuleFor(x => x.Dto.Secret).NotEmpty();
        RuleFor(x => x.Dto.Ticker).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.Action).NotEmpty().Must(a => new[]{ "buy","sell","close","hold" }.Contains(a));
        RuleFor(x => x.Dto.Price).GreaterThan(0);
        RuleFor(x => x.Dto.Ts).GreaterThan(0);
    }
}
