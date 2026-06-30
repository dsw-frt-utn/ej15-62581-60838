using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Dsw2026Ej15.Data.Dtos;

namespace Dsw2026Ej15.Data
{
    public class PersistenceEf : IPersistence
    {
        private readonly Dsw2026Ej15DbContext _context;

        public PersistenceEf(Dsw2026Ej15DbContext context)
        {
            _context = context;
            
            // Garantizar que la base de datos y las tablas se creen
            _context.Database.EnsureCreated();
            
            // Carga inicial de especialidades si la BD está vacía (Seed)
            SeedSpecialities();
        }

        public Speciality? GetSpecialityById(Guid id)
        {
            return _context.Specialities.Find(id);
        }

        public void AddDoctor(Doctor doctor)
        {
            // Para evitar que EF intente insertar de nuevo la especialidad si ya existe
            if (doctor.Speciality != null)
            {
                _context.Entry(doctor.Speciality).State = EntityState.Unchanged;
            }

            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }

        public IEnumerable<Doctor> GetActiveDoctors()
        {
            return _context.Doctors
                .Include(d => d.Speciality)
                .Where(d => d.IsActive)
                .ToList();
        }

        public Doctor? GetActiveDoctorById(Guid id)
        {
            return _context.Doctors
                .Include(d => d.Speciality)
                .FirstOrDefault(d => d.Id == id && d.IsActive);
        }

        public void DeactivateDoctor(Guid id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor != null)
            {
                doctor.Deactivate();
                _context.SaveChanges();
            }
        }

        private void SeedSpecialities()
        {
            if (!_context.Specialities.Any())
            {
                try
                {
                    string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sources", "specialities.json");
                    if (!File.Exists(jsonPath))
                    {
                        // Por si acaso no se copia al BaseDirectory pero está en el directorio del proyecto
                        jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Sources", "specialities.json");
                    }

                    if (File.Exists(jsonPath))
                    {
                        var json = File.ReadAllText(jsonPath);
                        var specialityDtos = JsonSerializer.Deserialize<List<SpecialityDto>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

                        var specialities = specialityDtos.Select(s => new Speciality(s.Name, s.Description, s.Id)).ToList();
                        
                        _context.Specialities.AddRange(specialities);
                        _context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Advertencia: No se encontró el archivo specialities.json para precargar los datos.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error al precargar especialidades: {e.Message}");
                }
            }
        }
    }
}
