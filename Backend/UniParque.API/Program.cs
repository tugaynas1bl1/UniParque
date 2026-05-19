using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UniParque.API.Extensions;
using UniParque_Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger()
                .AddUniParqueDbContext(builder.Configuration)
                .AddIdentityAndDb()
                .AddSettings(builder.Configuration)
                .AddJwtAuthenticationAndAuthorization(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapperAndOtherDI()
                .AddCorsPolicy();

var app = builder.Build();

await app.UseUniParquePipelineAsync();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    const int maxAttempts = 12;
    var delay = TimeSpan.FromSeconds(5);

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UniParqueDBContext>();

            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(
                ex,
                "Database is not ready yet. Retrying migrations in {DelaySeconds} seconds (attempt {Attempt}/{MaxAttempts}).",
                delay.TotalSeconds,
                attempt,
                maxAttempts);

            await Task.Delay(delay);
        }
    }

    throw new InvalidOperationException("Database migrations failed after multiple startup attempts.");
}

await ApplyMigrationsAsync(app);

app.Run();
