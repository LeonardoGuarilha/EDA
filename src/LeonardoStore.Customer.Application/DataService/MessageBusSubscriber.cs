using System;
using System.Threading;
using System.Threading.Tasks;
using LeonardoStore.Customer.Application.Commands;
using LeonardoStore.Customer.Application.Commands.Handlers;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.IntegrationEvents;
using LeonardoStore.SharedContext.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeonardoStore.Customer.Application.DataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageBus _bus;
        
        public MessageBusSubscriber(IMessageBus bus, IServiceScopeFactory scopeFactory)
        {
            _bus = bus;
            _scopeFactory = scopeFactory;
        }
        
        private void SetResponder()
        {
            _bus.RespondAsync<UserRegisteredEvent, CommandResult>(async request =>
                await RegisterCustomer(request));

            // O evento Connected é disparado quando o RabbitMQ está conectado a aplicação.
            _bus.AdvancedBus.Connected += OnConnect;
        }
        
        // No OnConnect eu registro novamente a ideia de que eu estou esperando alguma coisa
        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }

        private async Task<CommandResult> RegisterCustomer(UserRegisteredEvent userRegisteredEvent)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                using (var customerCommandHandler = _scopeFactory.CreateScope())
                {
                    var customerCommandTeste =
                        customerCommandHandler.ServiceProvider.GetRequiredService<CustomerCommandHandler>();

                    var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
                    //var message = JsonSerializer.Deserialize<UserRegisteredEvent>(userRegisteredEvent);

                    try
                    {
                        if (repository.EmailExists(userRegisteredEvent.Email) != null)
                        {
                            var createCustomerCommand = new CreateCustomerCommand();
                            createCustomerCommand.FirstName = userRegisteredEvent.FirstName;
                            createCustomerCommand.LastName = userRegisteredEvent.LastName;
                            createCustomerCommand.Document = userRegisteredEvent.Document;
                            createCustomerCommand.Email = userRegisteredEvent.Email;

                            await customerCommandTeste.HandleAsync(createCustomerCommand);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            return new CommandResult(true, "Cliente cadastrado com sucesso!", null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           SetResponder();
            return Task.CompletedTask;
        }
        
    }
}