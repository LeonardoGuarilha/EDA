using System.Threading.Tasks;
using Flunt.Notifications;
using LeonardoStore.Identity.Api.Services;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.Commands.Handler;

namespace LeonardoStore.Identity.Api.Commands.Handlers
{
    public class UserToRoleHandler :
        Notifiable,
        ICommandHandler<UserToRoleCommand>
    {
        private readonly AuthenticationAuthorizationService _authenticationAuthorizationService;

        public UserToRoleHandler(AuthenticationAuthorizationService authenticationAuthorizationService)
        {
            _authenticationAuthorizationService = authenticationAuthorizationService;
        }

        public async Task<CommandResult> HandleAsync(UserToRoleCommand command)
        {
            // Fail Fast Validation
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(
                    false, 
                    "Não foi possível adicionar a role ao cliente, favor verificar", 
                    command.Notifications);
            }

            var customer = await _authenticationAuthorizationService.UserManager.FindByEmailAsync(command.Email);
            
            if (customer == null)
                AddNotification("Customer", "Cliente não encontrado");
            
            var role = await _authenticationAuthorizationService.RoleManager.FindByNameAsync(command.Role);
            
            if (role == null)
                AddNotification("Role", "Role não encontrada");
            
            if (Invalid)
                return new CommandResult(
                    false, 
                    "Não foi possível adicionar a role ao cliente, favor verificar", 
                    Notifications);

            await _authenticationAuthorizationService.UserManager.AddToRoleAsync(customer, role.Name);

            return new CommandResult(true, "Role adicionada com sucesso", null);
        }
    }
}