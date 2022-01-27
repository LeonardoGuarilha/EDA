using Flunt.Notifications;
using Flunt.Validations;
using LeonardoStore.SharedContext.Commands;

namespace LeonardoStore.Customer.Application.Commands
{
    public class CreateRoleCommand : Notifiable, ICommand
    {
        public string RoleName { get; set; }
        
        public void Validate()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(RoleName, "RoleName", "Nome da role é obrigatório")
            );
        }
    }
}