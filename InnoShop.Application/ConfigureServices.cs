using System.Globalization;
using System.Reflection;
using InnoShop.Application.Middleware;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace InnoShop.Application;

public static class ConfigureServices {
    public static IServiceCollection AddApplicationServices<T>(this IServiceCollection services)
    where T : ICommandHandler {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.InvariantCulture;

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddTransient<ExceptionHandlingMiddleware>();

        var descriptors = assembly.GetTypes()
            .Where(typeof(ICommandHandler).IsAssignableFrom)
            .Where(x => !typeof(T).IsAssignableFrom(x))
            .Select(type => services.SingleOrDefault(desc => desc.ImplementationType == type))
            .Where(x => x != null)
            .ToList();

        descriptors.ForEach(desc => services.Remove(desc!));

        return services;
    }

    public static IServiceCollection ConfigureSwaggerJwt(this IServiceCollection services) {
        services.ConfigureSwaggerGen(options => {
            options.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Description = "Please provide a valid token",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder AddInnoshopApplicationMiddleware(this IApplicationBuilder builder) {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
