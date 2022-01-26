using Flunt.Notifications;
using Flunt.Validations;
using LeonardoStore.SharedContext.Commands;

namespace LeonardoStore.Customer.Application.Commands
{
    public class LoginCustomerCommand : Notifiable, ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        
        public void Validate()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsEmail(Email, "Email", "E-mail inválido")
                .IsNotNullOrEmpty(Email, "Email", "E-mail deve estar preenchido")
                .IsNotNullOrEmpty(Password, "Password", "Senha deve estar preenchida")
            );
        }
    }
}