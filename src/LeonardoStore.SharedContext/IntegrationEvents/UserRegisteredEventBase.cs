using System;

namespace LeonardoStore.SharedContext.IntegrationEvents
{
    public class UserRegisteredEvent : IntegrationEventsBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }

        protected UserRegisteredEvent() { }
        public UserRegisteredEvent (string firstName, string lastName, string document, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Document = document;
            Email = email;
        }
    }
}