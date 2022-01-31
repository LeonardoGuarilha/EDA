using LeonardoStore.Identity.Api.Commands.Handlers;
using LeonardoStore.Identity.Api.Configurations;
using LeonardoStore.Identity.Api.Services;
using LeonardoStore.SharedContext.IdentityConfig;
using LeonardoStore.SharedContext.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDevPack.Security.JwtSigningCredentials.AspNetCore;

namespace LeonardoStore.Identity.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment hostEnvironment)
        {
            // Para eu saber qual appsettings eu vou ler dependendo do ambiente
            var builder = new ConfigurationBuilder() //ConfigurationBuilder é IConfiguration
                .SetBasePath(hostEnvironment.ContentRootPath) // Pegar o caminho da aplicação
                .AddJsonFile("appsettings.json", true, true) // Adiciona o arquivo appsettings.json na configuração
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true) // Adiciono o appsettings dependendodo ambiente que eu estiver
                .AddEnvironmentVariables();

            // if (hostEnvironment.IsDevelopment())
            // {
            //     builder.AddUserSecrets<Startup>();
            // }

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityConfiguration(Configuration);
            
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            services.AddScoped<CreateRoleCommandHandler>();
            services.AddScoped<UserToRoleHandler>();
            services.AddScoped<AuthenticationAuthorizationService>();
            services.AddMessageBus(Configuration.GetMessageQueueConnection("MessageBus"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors("Total");

            app.UseAuthConfiguration();
            
            app.UseJwksDiscovery(); // Faço a exposição da chave pública

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}