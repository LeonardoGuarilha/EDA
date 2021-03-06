using System;
using System.Threading.Tasks;
using Flunt.Notifications;
using LeonardoStore.Customer.Domain.Enums;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.Customer.Domain.ValueObjects;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.Commands.Handler;


namespace LeonardoStore.Customer.Application.Commands.Handlers
{
    public class CustomerCommandHandler : 
        Notifiable,
        ICommandHandler<CreateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            
        }

        public async Task<CommandResult> HandleAsync(CreateCustomerCommand command)
        {
            // Fail Fast Validation
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível criar o cliente", command.Notifications);
            }
            
            // Verifica se o documento já está cadastrado
            if(await _customerRepository.DocumentExists(command.Document))
                AddNotification("Document", "Este CPF já está em uso");
            
            // Verifica se o e-mail já está cadastrado
            if(await _customerRepository.EmailExists(command.Email))
                AddNotification("Email", "Este e-mail já está em uso");
            
            // Gera os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);

            // Gera as entidades
            var customer = new Domain.Entities.Customer(name, document, email);
            //var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            
            // Agrupar as validações
            AddNotifications(name, document, email, customer);
            
            // Verifica as notificações de Document e Email
            if(Invalid)
                return new CommandResult(false, "Não foi possível realizar seu cadastro, verifique as informações", Notifications);
            
                // Salvar as informações
            _customerRepository.SaveCustomer(customer);
        
            await _customerRepository.UnitOfWork.Commit();

            // Envia e-mail de boas vindas
            customer.OnCustomerCreatedEvent += OnCustomerCreatedEvent;
            customer.CustomerCreatedEvent();

            // Retornar informações
            return new CommandResult(
                true, 
                "Cliente criado com sucesso!!", 
                new
                {
                    Id = customer.Id,
                    Name = customer.Name.ToString(),
                });
            //}

            //return new CommandResult(false, "Não foi possível realizar seu cadastro, verifique as informações", Notifications);
        }
        
        private void OnCustomerCreatedEvent(object sender, EventArgs args)
        {
            // Enviar e-mail de boas vindas
            Console.WriteLine("Bem vindo a Leonardo Store!");
        }
    }
}