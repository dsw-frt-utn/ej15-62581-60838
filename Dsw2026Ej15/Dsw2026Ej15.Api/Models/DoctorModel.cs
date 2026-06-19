using Dsw2026Ej15.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Ej15.Api.Models
{
    public record DoctorModel
    {
        public record Request(string Name, string LicenseNumber, Guid SpecialityId);
        public record Response(string Name, string LicenseNumber, string SpecialityName);
    }
}
