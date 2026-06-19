using Dsw2026Ej15.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Dsw2026Ej15.Domain.Interfaces
{
    public interface IPersistence
    {
        void AddDoctor(Doctor doctor);
        IEnumerable<Doctor> GetActiveDoctors();
        Doctor? GetDoctorById(Guid id);
        Speciality? GetSpecialityById(Guid id);
    }
}
