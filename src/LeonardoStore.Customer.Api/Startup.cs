using LeonardoStore.Customer.Application.Commands.Handlers;
using LeonardoStore.Customer.Application.DataService;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.Customer.Infra.DataContexts;
using LeonardoStore.Customer.Infra.Repositories;
using LeonardoStore.SharedContext.IdentityConfig;
using LeonardoStore.SharedContext.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeonardoStore.Customer.Api
{
    public class Startup
    {
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddIdentityConfiguration(Configuration);

            services.AddDbContext<CustomerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
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
            
            services.AddJwtConfiguration(Configuration);

            services.AddScoped<CustomerDbContext>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<CustomerCommandHandler>();
            //services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddHostedService<MessageBusSubscriber>();
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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}