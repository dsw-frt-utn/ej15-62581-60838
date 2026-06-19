using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Dsw2026Ej15.Data
{
    public class PersistenceInMemory : IPersistence
    {
        private readonly List<Doctor> _doctors = new();
        private List<Speciality> _specialities = new();

        public PersistenceInMemory()
        {
            LoadSpecialities();
        }

        private void LoadSpecialities()
        {
            try
            {
                var json = File.ReadAllText("specialities.json");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _specialities = JsonSerializer.Deserialize<List<Speciality>>(json, options) ?? new List<Speciality>();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error cargando especialidades: {e.Message}");
            }
        }

        public IEnumerable<Doctor> GetActiveDoctors()
        {
            return _doctors.Where(d => d.IsActive);
        }

        public Doctor? GetActiveDoctorById(Guid id)
        {
            return _doctors.FirstOrDefault(d => d.Id == id && d.IsActive);
        }

        public void AddDoctor(Doctor doctor)
        {
            _doctors.Add(doctor);
        }

        public void DeactivateDoctor(Guid id)
        {
            var doctor = GetActiveDoctorById(id);
            if (doctor != null)
            {
                doctor.IsActive = false;
            }
        }

        public Speciality? GetSpecialityById(Guid id)
        {
            return _specialities.FirstOrDefault(s => s.Id == id);
        }
    }
}
