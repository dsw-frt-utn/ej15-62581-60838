using Microsoft.AspNetCore.Mvc;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Api.Dtos;
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

        
        [HttpPost]
        public IActionResult CreateDoctor([FromBody] CreateDoctorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var speciality = _persistence.GetSpecialityById(request.SpecialityId);
            if (speciality == null)
            {
                return BadRequest(new { message = $"La especialidad con ID '{request.SpecialityId}' no existe." });
            }

            var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
            _persistence.AddDoctor(doctor);

            
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }

        
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

