using System.Globalization;
using System.Reflection;
using InnoShop.Application.Middleware;
using InnoShop.Application.Shared.Interfaces;
using InnoShop.Application.Shared.Misc;
using InnoShop.Application.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace InnoShop.Application;

public static class ConfigureServices {
    public static IServiceCollection AddApplicationServices<T>(this IServiceCollection services, IConfiguration config)
    where T : ICommandHandler {

        //FIXME: allow all hosts just for example 
        services.AddCors(options => {
            options.AddPolicy(name: "any",
                              policy => {
                                  policy.AllowAnyHeader();
                              });
        });

        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = ctx => {
                var ext = ctx.ProblemDetails.Extensions;

                if (!ext.ContainsKey("traceId")) {
                    ext.Add("traceId", ctx.HttpContext.TraceIdentifier);
                }
                if (!ext.ContainsKey("instance")) {
                    ext.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
                }
            });

        services.AddExceptionHandler<ExceptionHandler>();

        AddAuthorizationAndAuthentication(services, config);

        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.InvariantCulture;

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        var descriptors = assembly.GetTypes()
            .Where(typeof(ICommandHandler).IsAssignableFrom)
            .Where(x => !typeof(T).IsAssignableFrom(x))
            .Select(type => services.SingleOrDefault(desc => desc.ImplementationType == type))
            .Where(x => x != null)
            .ToList();

        descriptors.ForEach(desc => services.Remove(desc!));

        return services;
    }

    private static void AddAuthorizationAndAuthentication(IServiceCollection services, IConfiguration config) {
        services.AddAuthorization();

        services.AddAuthentication(options => {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.GetOrThrow("JwtIssuer"),
                ValidAudience = config.GetOrThrow("JwtAudience"),
                IssuerSigningKey = config.GetSecurityKey(),
            };
        });
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

            options.SchemaFilter<EnumSchemaFilter>();
        });

        return services;
    }

    public static IApplicationBuilder AddApplicationLayers(this IApplicationBuilder builder) {
        //FIXME: allow all hosts just for example 
        builder.UseCors("any");
        builder.UseExceptionHandler();
        builder.UseStatusCodePages();
        return builder;
    }
}
