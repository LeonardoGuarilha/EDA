using System;
using LeonardoStore.SharedContext.Entities;

namespace LeonardoStore.Customer.Domain.Entities
{
    public class Address : Entity
    {
        public Guid CustomerId { get; private set; }
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string Neighborhood { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        public Customer Customer { get; private set; }

        //public Customer Customer { get; private set; }
        
        protected Address() { }

        public Address(string street, string number, string neighborhood, string city, string state, string country, string zipCode)
        {
            Street = street;
            Number = number;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }
    }
}