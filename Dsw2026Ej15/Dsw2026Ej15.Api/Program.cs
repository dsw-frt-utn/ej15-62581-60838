
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Data;
using Dsw2026Ej15.Api.Middlewares;

namespace Dsw2026Ej15.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IPersistence, PersistenceInMemory>();

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Habilitar el middleware de control de excepciones personalizado
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health-check");

            app.Run();
        }
    }
}

