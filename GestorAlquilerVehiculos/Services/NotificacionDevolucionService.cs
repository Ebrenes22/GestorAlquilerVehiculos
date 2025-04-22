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

                var entregas = await context.EntregasDevoluciones
                    .Include(e => e.Reserva)
                        .ThenInclude(r => r.ClienteReserva)
                    .Where(e => e.FechaDevolucion == mañana)
                    .ToListAsync(stoppingToken);

                Console.WriteLine($"[INFO] Ejecutando notificación a las {DateTime.Now}");

                foreach (var entrega in entregas)
                {
                    var cliente = entrega.Reserva.ClienteReserva;
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
        <p>Le recordamos que mañana, <strong>{entrega.FechaDevolucion:dd/MM/yyyy}</strong>, vence el plazo para devolver el vehículo que reservó.</p>

        <div class='details'>
            <p><strong>Número de Reserva:</strong> #{entrega.ReservaID}</p>
            <p><strong>Vehículo:</strong> {entrega.Reserva.Vehiculo?.Marca} {entrega.Reserva.Vehiculo?.Modelo}</p>
            <p><strong>Fecha de Entrega:</strong> {entrega.FechaEntrega:dd/MM/yyyy}</p>
            <p><strong>Fecha de Devolución:</strong> {entrega.FechaDevolucion:dd/MM/yyyy}</p>
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

                        await emailService.SendEmailAsync(cliente.CorreoElectronico, subject, body);
                    }
                }
            }

            //await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
