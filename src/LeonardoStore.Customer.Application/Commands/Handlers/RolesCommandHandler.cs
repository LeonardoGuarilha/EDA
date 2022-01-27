using System.Security.Claims;
using System.Threading.Tasks;
using Flunt.Notifications;
using LeonardoStore.Customer.Application.Services;
using LeonardoStore.SharedContext.Commands;
using LeonardoStore.SharedContext.Commands.Handler;
using Microsoft.AspNetCore.Identity;

namespace LeonardoStore.Customer.Application.Commands.Handlers
{
    public class RolesCommandHandler :
        Notifiable,
        ICommandHandler<CreateRoleCommand>,
        ICommandHandler<UserToRoleCommand>
    {
        private readonly AuthenticationAuthorizationService _authenticationAuthorizationService;

        public RolesCommandHandler(AuthenticationAuthorizationService authenticationAuthorizationService)
        {
            _authenticationAuthorizationService = authenticationAuthorizationService;
        }

        public async Task<ICommandResult> HandleAsync(CreateRoleCommand command)
        {
            // Fail Fast Validation
            command.Validate();
            if (command.Invalid)
                return new CommandResult(false, "Erro ao cadastrar role", command.Notifications);
            
            // Verifica se a role existe
            var roleExist = await _authenticationAuthorizationService.RoleManager.RoleExistsAsync(command.RoleName);
            
            if (roleExist)
                return new CommandResult(false, "Role já existe", command.Notifications);

            var role = await _authenticationAuthorizationService.RoleManager.CreateAsync(new IdentityRole(command.RoleName));

            if (role.Succeeded)
                return new CommandResult(
                    true, 
                    "Role cadastrada com sucesso", 
                    new
                    {
                        RoleName = command.RoleName
                    });
            
            return new CommandResult(false, "Erro ao cadastrar role", role.Errors);
        }

        public async Task<ICommandResult> HandleAsync(UserToRoleCommand command)
        {
            // Fail Fast Validation
            command.Validate();
            if (command.Invalid)
                return new CommandResult(false, "Erro ao atribuir role ao usuário", command.Notifications);

            // Retorna o usuário
            var user = await _authenticationAuthorizationService.UserManager.FindByEmailAsync(command.Email);
            
            if (user == null)
                AddNotification("User", "Usurário não encontrado");
            
            // Retorna a role
            var role = await _authenticationAuthorizationService.RoleManager.FindByNameAsync(command.Role);

            if (role == null)
             AddNotification("Role", "Role não encontrada");

            if (Invalid)
                return new CommandResult(false, "Erro ao atribuir role ao usuário", command.Notifications);
            
            await _authenticationAuthorizationService.UserManager.AddToRoleAsync(user, role.Name);

            return new CommandResult(true, "Role adicionada ao usuário!", null);
        }
    }
}