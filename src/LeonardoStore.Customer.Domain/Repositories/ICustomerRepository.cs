using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeonardoStore.Customer.Domain.Entities;
using LeonardoStore.SharedContext.Repositories;

namespace LeonardoStore.Customer.Domain.Repositories
{
    public interface ICustomerRepository : IRepository<Entities.Customer>
    {
        void SaveCustomer(Entities.Customer customer);
        void UpdateCustomer(Entities.Customer customer);
        void DeleteCustomer(Entities.Customer customer);
        Task<Entities.Customer> GetCustomerById(Guid customerId);
        Task<IEnumerable<Entities.Customer>> GetAllCustomers();
        Task<Entities.Customer> GetCustomerByEmail(string email);
        Task<bool> DocumentExists(string document);
        Task<bool> EmailExists(string email);
        
        void SaveAddress(Address address);
        void UpdateAddress(Address address);
        void DeleteAddress(Address address);
        Task<Address> GetAddressById(Guid addressId);
        Task<IEnumerable<Address>> GetAllAddresses();
    }
}