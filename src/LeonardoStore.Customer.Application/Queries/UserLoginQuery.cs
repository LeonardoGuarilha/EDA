using System;
using System.Collections.Generic;

namespace LeonardoStore.Customer.Application.Queries
{
    public class UserLoginQuery
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserToken UsuarioToken { get; set; }
    }

    public class UserToken
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserClaim> Claims { get; set; }
    }

    public class UserClaim
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
}