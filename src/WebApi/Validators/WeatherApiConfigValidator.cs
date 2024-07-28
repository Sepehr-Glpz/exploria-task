using FluentValidation;
using SGSX.Exploria.WebApi.ViewModels;

namespace SGSX.Exploria.WebApi.Validators;
public class WeatherApiConfigValidator : AbstractValidator<WeatherApiConfigViewModel>
{
    private const string IS_REQUIRED_MSG = "{PropertyName} is required!";

    public WeatherApiConfigValidator() : base()
    {
        RuleFor(c => c.WeatherApiBaseUrl)
            .NotEmpty()
            .WithMessage(IS_REQUIRED_MSG);

        RuleFor(c => c.WeatherApiPath)
            .NotNull()
            .WithMessage(IS_REQUIRED_MSG);
    }
}
