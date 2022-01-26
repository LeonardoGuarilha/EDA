using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeonardoStore.Customer.Domain.Entities;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.Customer.Infra.DataContexts;
using LeonardoStore.SharedContext.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeonardoStore.Customer.Infra.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;
        
        public void SaveCustomer(Domain.Entities.Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void UpdateCustomer(Domain.Entities.Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public void DeleteCustomer(Domain.Entities.Customer customer)
        {
            _context.Customers.Remove(customer);
        }

        public async Task<Domain.Entities.Customer> GetCustomerById(Guid customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<IEnumerable<Domain.Entities.Customer>> GetAllCustomers()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        public async Task<Domain.Entities.Customer> GetCustomerByEmail(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email.Address == email);
        }

        public async Task<bool> DocumentExists(string document)
        {
            return await _context.Customers.AnyAsync(c => c.Document.Number == document);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _context.Customers.AnyAsync(c => c.Email.Address == email);
        }

        public void SaveAddress(Address address)
        {
            _context.Addresses.Add(address);
        }

        public void UpdateAddress(Address address)
        {
            _context.Addresses.Update(address);
        }

        public void DeleteAddress(Address address)
        {
            _context.Addresses.Remove(address);
        }

        public async Task<Address> GetAddressById(Guid addressId)
        {
            return await _context.Addresses.FindAsync(addressId);
        }

        public async Task<IEnumerable<Address>> GetAllAddresses()
        {
            return await _context.Addresses.AsNoTracking().ToListAsync();
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}