using FluentValidation;
using SGSX.Exploria.WebApi.ViewModels;

namespace SGSX.Exploria.WebApi.Validators;
public class DatabaseConfigValidator : AbstractValidator<DatabaseConfigViewModel>
{
    private const string IS_REQUIRED_MSG = "{PropertyName} is required!";

    public DatabaseConfigValidator() : base()
    {
        RuleFor(c => c.DefaultConnectionString)
            .NotEmpty()
            .WithMessage(IS_REQUIRED_MSG);
    }
}
