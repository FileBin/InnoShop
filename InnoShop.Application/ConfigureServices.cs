using System.Globalization;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.Application;

public static class ConfigureServices {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        return services;
    }
}
