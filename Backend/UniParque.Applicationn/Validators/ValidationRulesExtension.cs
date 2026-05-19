using FluentValidation;
using System.Text.RegularExpressions;

namespace UniParque.Application.Validators;

public static class ValidationRulesExtension
{
    public static IRuleBuilder<T, string> Password<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        bool mustContainLowerCase = true,
        bool mustContainUpperCase = true,
        bool mustContainDigit = true,
        bool mustContainSpecialSymbol = true)
    {
        return ruleBuilder
            .Must(password =>
            {
                if (mustContainLowerCase && !Regex.IsMatch(password, @"[a-z]"))
                    return false;
                if (mustContainUpperCase && !Regex.IsMatch(password, @"[A-Z]"))
                    return false;
                if (mustContainDigit && !Regex.IsMatch(password, @"\d"))
                    return false;
                return true;
            }).WithMessage("Invalid password");
    }
}
