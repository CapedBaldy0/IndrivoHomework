using FluentValidation;

namespace IndrivoHomework.Models;

public class UserModelFluentValidator : AbstractValidator<UserModel>
{
    public List<string> UsedUsernames { get; set; } = new();

    public UserModelFluentValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<UserModel>.CreateWithOptions((UserModel)model, x => x.IncludeProperties(propertyName)));

        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
