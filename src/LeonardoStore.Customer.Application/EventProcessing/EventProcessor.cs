using System;
using System.Text.Json;
using System.Threading.Tasks;
using LeonardoStore.Customer.Application.Commands;
using LeonardoStore.Customer.Application.Commands.Handlers;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.SharedContext.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace LeonardoStore.Customer.Application.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task ProcessEvent(string message)
        {
            await AddCustomer(message);
        }

        private async Task AddCustomer(string userRegisteredEvent)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                using (var customerCommandHandler = _scopeFactory.CreateScope())
                {
                    var customerCommandTeste =
                        customerCommandHandler.ServiceProvider.GetRequiredService<CustomerCommandHandler>();
                    
                    var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
                    var message = JsonSerializer.Deserialize<UserRegisteredEvent>(userRegisteredEvent);

                    try
                    {
                        if (repository.EmailExists(message.Email) != null)
                        {
                            var createCustomerCommand = new CreateCustomerCommand();
                            createCustomerCommand.FirstName = message.FirstName;
                            createCustomerCommand.LastName = message.LastName;
                            createCustomerCommand.Document = message.Document;
                            createCustomerCommand.Email = message.Email;

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
            
        }
    }
}