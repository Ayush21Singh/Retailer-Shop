using AshishGeneralStore.Data;
using System.Reflection;

namespace AshishGeneralStore.Extensions
{
    public static class ServiceRegistration
    {
        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var typesWithInterfaces = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Implementation = t,
                    Interface = t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}")
                    // Matches IAuthService -> AuthService, IUserService -> UserService, etc.
                })
                .Where(t => t.Interface != null);

            foreach (var type in typesWithInterfaces)
            {
                services.AddScoped(type.Interface, type.Implementation);
            }

            // Manually register singleton services
            services.AddSingleton<ElasticsearchService>();
        }
    }
}
