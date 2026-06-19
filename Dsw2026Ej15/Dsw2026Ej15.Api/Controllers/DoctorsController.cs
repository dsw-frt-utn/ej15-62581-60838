using Dsw2026Ej15.Api.Models;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateDoctor(DoctorModel.Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LicenseNumber))
            {
                throw new ValidationException("Nombre y Licencia requeridos");
            }

            var speciality = _persistence.GetSpecialityById(request.SpecialityId);
            if (speciality == null)
            {
                throw new ValidationException("La especialidad no existe");
            }

            var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
            _persistence.AddDoctor(doctor);

            return Created();
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveDoctors()
        {
            var activeDoctors = _persistence.GetActiveDoctors();
            var response = activeDoctors.Select(d => new DoctorModel.Response(d.Name, d.LicenseNumber, d.Speciality.Name));

            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetActiveDoctorsById(Guid id)
        {
            var doctor = _persistence.GetActiveDoctorById(id);

            if (doctor == null) return NotFound("No se encontro el medico o inactivo");

            var response = new DoctorModel.Response(doctor.Name, doctor.LicenseNumber, doctor.Speciality.Name);

            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteDoctors(Guid id)
        {
            var doctor = _persistence.GetActiveDoctorById(id);

            if (doctor == null) return NotFound(new { Error = "Médico no encontrado o inactivo." });

            _persistence.DeactivateDoctor(id);

            return NoContent();
        }
    }
}
