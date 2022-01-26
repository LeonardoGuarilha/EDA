using Flunt.Notifications;
using Flunt.Validations;
using LeonardoStore.Customer.Domain.Enums;
using LeonardoStore.SharedContext.Commands;

namespace LeonardoStore.Customer.Application.Commands
{
    public class CreateCustomerCommand : Notifiable, ICommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Document { get; set; }
        public EDocumentType DocumentType { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string Neighborhood { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }
        
        public void Validate()
        {
            // Fail fast validation
            // TODO: Adicionar as outras validações
            AddNotifications(new Contract()
                .Requires()
                .HasMinLen(FirstName, 3, "Name.FirstName", "Nome deve conter pelo menos 3 caracteres")
                .HasMinLen(LastName, 3, "Name.LastName", "Sobrenome deve conter pelo menos 3 caracteres")
                .HasMaxLen(FirstName, 40, "Name.FirstName", "Nome deve conter até 40 caracteres")
                .IsNotNullOrEmpty(Document, "Document", "CPF/CNPJ é obrigatório")
            );
        }
    }
}