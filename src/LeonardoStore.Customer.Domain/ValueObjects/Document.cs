using Flunt.Validations;
using LeonardoStore.Customer.Domain.Enums;
using LeonardoStore.SharedContext.ValueObjects;

namespace LeonardoStore.Customer.Domain.ValueObjects
{
    public class Document : ValueObject
    {
        public string Number { get; private set; }
        public EDocumentType Type { get; private set; }

        public Document(string number, EDocumentType type)
        {
            Number = number;
            Type = type;
            
            AddNotifications(new Contract()
                .Requires()
                .IsTrue(Validate(), "Document.Number", "Documento inválido")
            );
        }
        
        private bool Validate()
        {
            // Fazer toda a validação de CPF/CNPJ
            if (Type == EDocumentType.CNPJ && Number.Length == 14)
                return true;
            
            if (Type == EDocumentType.CPF && Number.Length == 11)
                return true;

            return false;
        }
        
    }
}