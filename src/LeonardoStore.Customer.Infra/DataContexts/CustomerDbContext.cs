using System.Linq;
using System.Threading.Tasks;
using Flunt.Notifications;
using LeonardoStore.Customer.Domain.Entities;
using LeonardoStore.SharedContext.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeonardoStore.Customer.Infra.DataContexts
{
    public class CustomerDbContext : DbContext, IUnitOfWork
    {

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
            
        }

        public DbSet<Domain.Entities.Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Pega tudo que é string e coloca pra varchar(100) caso não esteja declarado
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                         e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.Ignore<Notification>();
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
        }

        public Task<bool> Commit()
        {
            throw new System.NotImplementedException();
        }
    }
}