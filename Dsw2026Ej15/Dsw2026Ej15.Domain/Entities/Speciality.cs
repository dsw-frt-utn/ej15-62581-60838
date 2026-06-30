using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Domain.Entities
{
    public class Speciality : BaseEntity
    {
        public string Name { get; init; }
        public string Description { get; init; }

        public Speciality(string name, string description, Guid? id = null) : base(id)
        {
            Name = name;
            Description = description;
        }

        #pragma warning disable CS8618 // Required by EF Core
        protected Speciality() { }
        #pragma warning restore CS8618
    }
}
