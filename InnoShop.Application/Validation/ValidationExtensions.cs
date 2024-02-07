namespace InnoShop.Application.Validation;

internal static class ValidationExtensions {
    public static IRuleBuilderOptions<T, string> PasswordValidation<T>(this IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder.NotEmpty().Length(8,256);
    }

    public static IRuleBuilderOptions<T, string> UsernameValidation<T>(this IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder.LoginValidation().Matches("^[A-Za-z0-9_\\.-]{3,30}$");
    }

    public static IRuleBuilderOptions<T, string> EmailValidation<T>(this IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder.LoginValidation().Matches("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,10}$");
    }

    public static IRuleBuilderOptions<T, string> LoginValidation<T>(this IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder.NotEmpty().MaximumLength(80);
    }
}