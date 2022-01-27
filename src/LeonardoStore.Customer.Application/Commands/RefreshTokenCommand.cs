using Flunt.Notifications;
using Flunt.Validations;
using LeonardoStore.SharedContext.Commands;

namespace LeonardoStore.Customer.Application.Commands
{
    public class RefreshTokenCommand : Notifiable, ICommand
    {
        public string RefreshToken { get; set; }
        
        public void Validate()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(RefreshToken, "RefreshToken", "RefreshToken não pode ser vazio")
            );
        }
    }
}