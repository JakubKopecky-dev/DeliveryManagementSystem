using Microsoft.EntityFrameworkCore;
using CourierService.Persistence;

namespace CourierService.Api.Auth
{
    public static class MigrationApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CourierDbContext>();
            db.Database.Migrate();
        }
    }
}
