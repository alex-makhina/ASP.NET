using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Pcf.GivingToCustomer.Core.Domain
{
    public class Customer
        :BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public List<Guid> PreferenceIds { get; set; } = new List<Guid>();
        
        public List<Guid> PromoCodeIds { get; set; } = new List<Guid>();
    }
}