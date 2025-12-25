using Microsoft.EntityFrameworkCore;
using DeliveryService.Command.Persistence;

namespace DeliveryService.Command.Api.DependencyInjection
{
    public static class MigrationApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            db.Database.Migrate();
        }
    }
}
