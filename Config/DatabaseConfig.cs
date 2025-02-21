using Microsoft.EntityFrameworkCore;
using AshishGeneralStore.Data;
using System;

namespace AshishGeneralStore.Config
{
    public static class DatabaseConfig
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36))));
        }
    }
}
