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

            var speciality = await _persistence.GetSpecialityById(request.SpecialityId);
            if (speciality == null)
            {
                throw new ValidationException("La especialidad no existe");
            }

            var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
             await _persistence.AddDoctor(doctor);

            return Created();
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveDoctors()
        {
            var activeDoctors = await _persistence.GetAllDoctors();
            var response = activeDoctors.Where(d => d.IsActive).Select(d => new DoctorModel.Response(d.Id, d.Name,
                d.LicenseNumber, d.Speciality?.Name)).ToList();

            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetActiveDoctorsById(Guid id)
        {
            var doctor = await _persistence.GetAllDoctors();

            if (doctor == null) return NotFound("No se encontro el medico o inactivo");

            var response = doctor.Where(d => d.IsActive).Select(d => new DoctorModel.Response(d.Id, d.Name,
               d.LicenseNumber, d.Speciality?.Name)).ToList();

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctors(Guid id)
        {
            var doctor = (await GetDoctor(id))!;         

            doctor.Deactivate();
            await _persistence.UpdateDoctor(doctor);

            return NoContent();
        }

        private async Task<Doctor?> GetDoctor(Guid id)
        {
            return await _persistence.GetDoctor(id) ?? throw new EntityNotFoundException("El medico solicitado no existe o no esta activo. ");
        }
    }
}
