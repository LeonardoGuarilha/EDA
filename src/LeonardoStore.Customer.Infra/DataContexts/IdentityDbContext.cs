using LeonardoStore.Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.JwtSigningCredentials;
using NetDevPack.Security.JwtSigningCredentials.Store.EntityFrameworkCore;

namespace LeonardoStore.Customer.Infra.DataContexts
{
    public class IdentityDbContext: 
        Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext, ISecurityKeyContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
        
        public DbSet<SecurityKeyWithPrivate> SecurityKeys { get; set; } // Salva as chaves priadas do JWK

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}