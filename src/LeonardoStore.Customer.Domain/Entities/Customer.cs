using System;
using System.Collections.Generic;
using LeonardoStore.Customer.Domain.ValueObjects;
using LeonardoStore.SharedContext.Entities;

namespace LeonardoStore.Customer.Domain.Entities
{
    public class Customer : Entity, IAggregateRoot
    {
        public Name Name { get; private set; }
        public Document Document { get; private set; }
        public Email Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdateDate { get; private set; }

        public ICollection<Address> Addresses { get; set; }

        public event EventHandler OnCustomerCreatedEvent;
        
        protected Customer(DateTime createdAt, DateTime lastUpdateDate)
        {
            CreatedAt = createdAt;
            LastUpdateDate = lastUpdateDate;
        }

        public Customer(Name name, Document document, Email email)
        {
            Name = name;
            Document = document;
            Email = email;
            CreatedAt = DateTime.Now;

            AddNotifications(name, document, email);
        }
        
        // public void AddBillingAddres(Address billingAddress)
        // {
        //     BillingAddress = billingAddress;
        // }
        //
        // public void AddShippingAddres(Address shippingAddress)
        // {
        //     ShippingAddress = shippingAddress;
        // }

        public void CustomerCreatedEvent()
        {
            LastUpdateDate = DateTime.Now;
            
            var args = EventArgs.Empty;
            var handler = OnCustomerCreatedEvent;
            handler?.Invoke(this, args);
        }
    }
}