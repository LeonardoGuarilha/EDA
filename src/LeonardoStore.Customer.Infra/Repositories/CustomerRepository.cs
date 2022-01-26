using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeonardoStore.Customer.Domain.Entities;
using LeonardoStore.Customer.Domain.Repositories;
using LeonardoStore.SharedContext.Repositories;

namespace LeonardoStore.Customer.Infra.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        public void SaveCustomer(Domain.Entities.Customer customer)
        {
            throw new NotImplementedException();
        }

        public void UpdateCustomer(Domain.Entities.Customer customer)
        {
            throw new NotImplementedException();
        }

        public void DeleteCustomer(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public Domain.Entities.Customer GetCustomerById(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Customer>> GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DocumentExists(string document)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailExists(string email)
        {
            throw new NotImplementedException();
        }

        public void SaveAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public void UpdateAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public void DeleteAddress(Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Address GetAddressById(Guid addressId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Address>> GetAllAddresses()
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}