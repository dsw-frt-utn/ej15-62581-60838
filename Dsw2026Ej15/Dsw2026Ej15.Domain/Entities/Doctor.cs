using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public string Name { get; init; }
        public string LicenseNumber { get; init; }
        public bool IsActive { get; private set; }
        public Speciality Speciality { get; private set; }

        public Doctor(string name, string license, Speciality speciality, Guid? id = null) : base(id)
        {
            Name = name;
            LicenseNumber = license;
            Speciality = speciality;
            IsActive = true;
        }

        #pragma warning disable CS8618 // Required by EF Core
        protected Doctor() { }
        #pragma warning restore CS8618

        public void Deactivate()
        {
            IsActive = false;
        }

    }
}
