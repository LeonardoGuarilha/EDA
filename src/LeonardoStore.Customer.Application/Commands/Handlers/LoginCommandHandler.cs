using System.Threading.Tasks;
using Flunt.Notifications;
using LeonardoStore.Customer.Application.Services;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.Commands.Handler;

namespace LeonardoStore.Customer.Application.Commands.Handlers
{
    public class LoginCommandHandler :
        Notifiable,
        ICommandHandler<LoginCustomerCommand>
    {
        
        private readonly ICustomerRepository _customerRepository;
        private readonly AuthenticationAuthorizationService _authenticationAuthorizationService;

        public LoginCommandHandler(ICustomerRepository customerRepository, AuthenticationAuthorizationService authenticationAuthorizationService)
        {
            _customerRepository = customerRepository;
            _authenticationAuthorizationService = authenticationAuthorizationService;
        }

        public async Task<ICommandResult> HandleAsync(LoginCustomerCommand command)
        {
            // Fail Fast Validation
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Falha ao logar", command.Notifications);
            }
            
            // Retorna o usuário
            var customer = await _customerRepository.GetCustomerByEmail(command.Email);
            
            if (customer == null)
                AddNotification("Customer", "Usuário não encontrado");
            
            // Verifica as notificações
            if(Invalid)
                return new CommandResult(false, "Usuário ou senha incorretos", null);

            // Verifico usuário e senha
            var result = await _authenticationAuthorizationService.SignInManager.PasswordSignInAsync(
                command.Email, command.Password, false, false);

            if (result.Succeeded)
            {
                return new CommandResult(
                    true,
                    "Login realizado com sucesso",
                    new
                    {
                      jwt = _authenticationAuthorizationService.CreateJwt(command.Email, customer.Id)
                    });
            }
            
            if(result.IsLockedOut)
                AddNotification("Customer","Usuário temporariamente bloqueado por tentativas inválidas");
            
            return new CommandResult(false, "Usuário ou senha incorretos", null);
        }
    }
}