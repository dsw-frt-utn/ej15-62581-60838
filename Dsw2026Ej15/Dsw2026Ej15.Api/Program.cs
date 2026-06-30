
using Dsw2026Ej15.Domain.Interfaces;
using Dsw2026Ej15.Data;
using Dsw2026Ej15.Api.Middlewares;
using Microsoft.EntityFrameworkCore;

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

            // Configurar DbContext con SQLite
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Dsw2026Ej15DbContext>(options =>
                options.UseSqlite(connectionString));

            // Registrar la persistencia con Entity Framework
            builder.Services.AddScoped<IPersistence, PersistenceEf>();

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Forzar la creación de la base de datos y carga inicial de datos en el inicio
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Al resolver IPersistence, se ejecuta el constructor de PersistenceEf,
                    // el cual asegura la creación de la base de datos y realiza el seeding.
                    var persistence = services.GetRequiredService<IPersistence>();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n========================================================");
                    Console.WriteLine("❌ ERROR AL INICIALIZAR LA BASE DE DATOS:");
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Detalle: {ex.InnerException.Message}");
                    }
                    Console.WriteLine("========================================================\n");
                    Console.ResetColor();
                    throw;
                }
            }

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

