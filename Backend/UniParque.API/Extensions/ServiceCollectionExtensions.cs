using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
using UniParque_Infrastructure.Persistence;
using UniParque_Infrastructure.Repositories;
using UniParque_Infrastructure.Services;

namespace UniParque.API.Extensions;

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

        services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection AddUniParqueDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        services.AddDbContext<UniParqueDBContext>(
            options => options.UseSqlServer(connectionString)
        );

        return services;
    }

    public static IServiceCollection AddIdentityAndDb(
        this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<UniParqueDBContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CloudinarySettings>(
            configuration.GetSection("CloudinarySettings")
        );

        services.Configure<EmailConfig>(
            configuration.GetSection("EmailSettings")
        );

        services.Configure<JwtConfig>(
            configuration.GetSection(JwtConfig.SectionName)
        );

        return services;
    }

    public static IServiceCollection AddJwtAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.GetSection("JwtSettings").Bind(jwtConfig);

        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                }
            );

        services.AddAuthorization(
            options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("AdminOrManager", policy => policy.RequireRole("Admin", "Manager"));
                options.AddPolicy("UserOrAbove", policy => policy.RequireRole("Admin", "Manager", "User"));
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

    public static IServiceCollection AddAutoMapperAndOtherDI(
        this IServiceCollection services)
    {
        services.AddScoped<IParkingBranchService, ParkingBranchService>();
        services.AddScoped<IParkingSectionService, ParkingSectionService>();
        services.AddScoped<IParkingSubSectionService, ParkingSubSectionService>();
        services.AddScoped<IParkingPlaceService, ParkingPlaceService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReservationService, ReservationService>();
        //services.AddScoped<IPhotoService, CloudinaryPhotoService>();
        services.AddScoped<IPhotoService, S3PhotoService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaymentService, PaymentService>();


        services.AddScoped<IParkingBranchRepository, ParkingBranchRepository>();
        services.AddScoped<IParkingSectionRepository, ParkingSectionRepository>();
        services.AddScoped<IParkingSubSectionRepository, ParkingSubSectionRepository>();
        services.AddScoped<IParkingPlaceRepository, ParkingPlaceRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserVerificationRepository, UserVerificationRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();

        services.AddSignalR();

        services.AddHostedService<ReservationCleanupService>();

        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                    "http://localhost:8080",
                    "http://54.145.24.21"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
