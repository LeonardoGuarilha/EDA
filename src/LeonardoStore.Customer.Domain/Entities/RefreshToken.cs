using System;

namespace LeonardoStore.Customer.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } // Serve somente para ser a chave primária da tabela
        public Guid UserId { get; set; }
        public string Username { get; set; } // Parametro de controle, vai ser o email do usuário logado
        public Guid Token { get; set; } // Precisa somente que seja um dado único
        public DateTime ExpirationDate { get; set; }

        public RefreshToken()
        {
            Id = Guid.NewGuid();
            Token = Guid.NewGuid();
        }
    }
}