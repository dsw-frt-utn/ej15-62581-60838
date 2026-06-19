using System;
using System.ComponentModel.DataAnnotations;

namespace Dsw2026Ej15.Api.Dtos
{
    public class CreateDoctorRequest
    {
        [Required(ErrorMessage = "El nombre del médico es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de licencia (matrícula) es requerido.")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Id de la especialidad es requerido.")]
        public Guid SpecialityId { get; set; }
    }
}
