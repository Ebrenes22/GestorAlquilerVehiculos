using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Services;

public class NotificacionDevolucionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _horaObjetivo = new TimeSpan(10, 10, 0);

    public NotificacionDevolucionService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ahora = DateTime.Now;
            var horaEjecucion = DateTime.Today.Add(_horaObjetivo);

            if (ahora > horaEjecucion)
                horaEjecucion = horaEjecucion.AddDays(1);

            var tiempoEspera = horaEjecucion - ahora;
            await Task.Delay(tiempoEspera, stoppingToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<GestorAlquilerVehiculosDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var mañana = DateTime.Today.AddDays(1);
                var inicio = mañana;
                var fin = mañana.AddDays(1);

                var reservasConDevolucionManana = await context.Reservas
                    .Include(r => r.ClienteReserva)
                    .Include(r => r.Vehiculo)
                    .Where(r => r.FechaFin >= inicio && r.FechaFin < fin && r.Estado != "Cancelada")
                    .ToListAsync(stoppingToken);

                Console.WriteLine($"[INFO] Ejecutando notificación a las {DateTime.Now}");

                foreach (var reserva in reservasConDevolucionManana)
                {
                    var cliente = reserva.ClienteReserva;
                    var vehiculo = reserva.Vehiculo;

                    if (!string.IsNullOrEmpty(cliente?.CorreoElectronico))
                    {
                        string subject = "Recordatorio: devolución de vehículo mañana";
                        string body = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        h1 {{ color: #333366; }}
        .details {{ margin-top: 20px; }}
        .details p {{ margin: 5px 0; }}
        .footer {{ margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Recordatorio de Devolución</h1>
        <p>Estimado/a {cliente.NombreCompleto},</p>
        <p>Le recordamos que mañana, <strong>{reserva.FechaFin:dd/MM/yyyy}</strong>, vence el plazo para devolver el vehículo que reservó.</p>

        <div class='details'>
            <p><strong>Número de Reserva:</strong> #{reserva.ReservaID}</p>
            <p><strong>Vehículo:</strong> {vehiculo.Marca} {vehiculo.Modelo}</p>
            <p><strong>Fecha de Inicio:</strong> {reserva.FechaInicio:dd/MM/yyyy}</p>
            <p><strong>Fecha de Devolución:</strong> {reserva.FechaFin:dd/MM/yyyy}</p>
        </div>

        <p>Por favor, asegúrese de devolver el vehículo a tiempo para evitar cargos adicionales.</p>

        <p>Gracias por utilizar nuestros servicios.</p>

        <div class='footer'>
            <p>Gestor de Alquiler de Vehículos</p>
            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
        </div>
    </div>
</body>
</html>";

                        try
                        {
                            await emailService.SendEmailAsync(cliente.CorreoElectronico, subject, body);
                            Console.WriteLine($"[INFO] Correo enviado a {cliente.CorreoElectronico}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] No se pudo enviar el correo: {ex.Message}");
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

}
