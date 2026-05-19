using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using UniParque.Application.Config;
using UniParque.Application.Helpers;
using UniParque.Application.Mappings;
using UniParque.Application.Repositories;
using UniParque.Application.Services;
using UniParque.Application.Validators;
using UniParque_Domain.Entities;

namespace UniParque.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddOpenApi();

        services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "UniParque API",
                        Description = "This API includes fully CRUD operations for the UniParque project",
                        Contact = new OpenApiContact
                        {
                            Name = "UniParque Team",
                            Email = "support@uniparque.com"
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("http://opensource.org/license/mit")
                        }
                    });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                        options.IncludeXmlComments(xmlPath);

                    // JWT options for Swagger
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = """
                JWT authorization header using the Bearer scheme.
                Example: Authorization: Bearer {token}
                """,
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                    Array.Empty<string>()
                }
                    });
                });
        return services;
    }

    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

}
