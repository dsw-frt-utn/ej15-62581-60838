using Microsoft.AspNetCore.Mvc;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Api.Dtos;
using Dsw2026Ej15.Domain.Exceptions;
using System;
using System.Linq;

namespace Dsw2026Ej15.Api.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly IPersistence _persistence;

        public DoctorsController(IPersistence persistence)
        {
            _persistence = persistence;
        }

        // i. POST api/doctors - Insertar un nuevo médico (lanza ValidationException)
        [HttpPost]
        public IActionResult CreateDoctor([FromBody] CreateDoctorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("El nombre del médico es requerido.");
            }

            if (string.IsNullOrWhiteSpace(request.LicenseNumber))
            {
                throw new ValidationException("El número de licencia (matrícula) es requerido.");
            }

            var speciality = _persistence.GetSpecialityById(request.SpecialityId);
            if (speciality == null)
            {
                throw new ValidationException($"La especialidad con ID '{request.SpecialityId}' no existe.");
            }

            var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
            _persistence.AddDoctor(doctor);

            // Retorna 201 Created y el enlace para consultar al nuevo médico
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }

        // ii. GET api/doctors - Obtener todos los médicos activos
        [HttpGet]
        public IActionResult GetActiveDoctors()
        {
            var doctors = _persistence.GetActiveDoctors();

            var result = doctors.Select(d => new ActiveDoctorDto
            {
                Id = d.Id,
                Name = d.Name,
                LicenseNumber = d.LicenseNumber,
                SpecialityName = d.Speciality.Name
            }).ToList();

            return Ok(result);
        }

        // iii. GET api/doctors/{id} - Obtener un médico activo a partir de su Id
        [HttpGet("{id:guid}")]
        public IActionResult GetDoctorById(Guid id)
        {
            var doctor = _persistence.GetDoctorById(id);
            if (doctor == null || !doctor.IsActive)
            {
                return NotFound();
            }

            var result = new DoctorResponse
            {
                Name = doctor.Name,
                LicenseNumber = doctor.LicenseNumber,
                SpecialityName = doctor.Speciality.Name
            };

            return Ok(result);
        }

        // iv. DELETE api/doctors/{id} - Establecer como inactivo al médico
        [HttpDelete("{id:guid}")]
        public IActionResult DeactivateDoctor(Guid id)
        {
            var doctor = _persistence.GetDoctorById(id);
            if (doctor == null || !doctor.IsActive)
            {
                return NotFound();
            }

            doctor.Deactivate();
            return NoContent();
        }
    }
}
