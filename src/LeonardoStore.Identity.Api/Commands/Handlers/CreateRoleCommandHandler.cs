using System.Linq;
using System.Threading.Tasks;
using Flunt.Notifications;
using LeonardoStore.Identity.Api.Services;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.Commands.Handler;
using Microsoft.AspNetCore.Identity;

namespace LeonardoStore.Identity.Api.Commands.Handlers
{
    public class CreateRoleCommandHandler :
        Notifiable,
        ICommandHandler<CreateRoleCommand>
    {
        
        private readonly AuthenticationAuthorizationService _authenticationAuthorizationService;

        public CreateRoleCommandHandler(AuthenticationAuthorizationService authenticationAuthorizationService)
        {
            _authenticationAuthorizationService = authenticationAuthorizationService;
        }

        public async Task<ICommandResult> HandleAsync(CreateRoleCommand command)
        {
            // Fail Fast Validation
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível criar o cliente", command.Notifications);
            }

            var roleExists = await _authenticationAuthorizationService.RoleManager.FindByNameAsync(command.RoleName);
            
            if (roleExists != null)
            {
                return new CommandResult(false, "Já existe uma role com esse nome", Notifications);
            }
            
            var result = await _authenticationAuthorizationService.RoleManager.CreateAsync(new IdentityRole(command.RoleName));

            if (result.Succeeded)
            {
                return new CommandResult(
                    true,
                    "Role criada com sucesso!",
                    new
                    {
                        RoleName = command.RoleName
                    });
            }
            
            return new CommandResult(
                false, 
                "Não foi possível criar a role", 
                result.Errors.Select(e => e.Description));
        }
    }
}