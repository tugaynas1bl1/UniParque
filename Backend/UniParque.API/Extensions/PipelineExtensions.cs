using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using UniParque.Application.Hubs;

namespace UniParque.API.Extensions;

using Microsoft.EntityFrameworkCore;
using UniParque_Infrastructure.Persistence;

public static class PipelineExtensions
{
    public static async Task<WebApplication> UseUniParquePipelineAsync(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            options.RoutePrefix = string.Empty;
            options.EnableFilter();
            options.EnableTryItOutByDefault();
            options.DisplayRequestDuration();
            options.EnablePersistAuthorization();
        });
        app.MapOpenApi();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<UniParqueDBContext>();
                await context.Database.MigrateAsync();
                await RoleSeeder.SeedRolesAsync(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed error: {ex.Message}");
            }
        }

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ReservationHub>("/hubs/reservation");

        return app;
    }
}