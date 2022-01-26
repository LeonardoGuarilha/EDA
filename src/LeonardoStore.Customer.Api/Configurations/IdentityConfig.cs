using LeonardoStore.Customer.Application.Configurations;
using LeonardoStore.Customer.Infra.DataContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Security.JwtSigningCredentials;

namespace LeonardoStore.Customer.Api.Configurations
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            
            //var appSettingsSection = configuration.GetSection("AppTokenSettings");
            // Vai retornar a instancia de AppTokenSettings com o que tem no appSettings > AppTokenSettings
            //services.Configure<AppTokenSettings>(appSettingsSection);
            
            // Adiciono o JWKS Manager
            services.AddJwksManager(options => options.Algorithm = Algorithm.ES256) // Escolhido o Algoritmo ES256
                .PersistKeysToDatabaseStore<IdentityDbContext>(); // A chave vai ser persistida no banco

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Configuração do Identity, o suporte ao Identity
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityPortugueseMessages>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders(); // Verificar no identity como que faz redefinição de senha

            // services.AddJwtConfiguration(configuration);

            return services;
        }
    }
}