using Flunt.Notifications;
using Flunt.Validations;
using LeonardoStore.SharedContext.Commands;

namespace LeonardoStore.Identity.Api.Commands
{
    public class UserToRoleCommand : Notifiable, ICommand
    {
        public string Email { get; set; }
        public string Role { get; set; }
        
        public void Validate()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Email, "Email", "E-mail é obrigatório")
                .IsNotNullOrEmpty(Role, "Role", "Role é obrigatório")
            );
        }
    }
}