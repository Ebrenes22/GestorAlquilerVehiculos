using GestorAlquilerVehiculos.Data;
using GestorAlquilerVehiculos.Models;
using GestorAlquilerVehiculos.Utils;
using Microsoft.AspNetCore.Mvc;

public class ReportesController : Controller
{
    private readonly GestorAlquilerVehiculosDbContext _context;

    public ReportesController(GestorAlquilerVehiculosDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public IActionResult PrintReporte(string reportType, DateTime? startDate, DateTime? endDate)
    {
        byte[] fileContent;
        string fileName = $"{reportType}_{DateTime.Now:yyyyMMdd}.xlsx";

        if (reportType == "EstadoFlota")
        {
            var lista = _context.Vehiculos.ToList();
            var cols = new Dictionary<string, Func<Vehiculo, object>>
              {
                  { "Marca", v => v.Marca },
                  { "Modelo", v => v.Modelo },
                  { "Placa", v => v.Placa },
                  { "Estado", v => v.Estado },
                  { "PrecioPorDia", v => v.PrecioPorDia }
              };
            fileContent = ExcelExportService.ExportToExcel<Vehiculo>(lista, cols, "EstadoFlota");
        }
        else if (reportType == "Ingresos")
        {
            var query = _context.Reservas.AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
                query = query.Where(r => r.FechaRegistro >= startDate && r.FechaRegistro <= endDate);

            var lista = query.Select(r => new
            {
                Fecha = r.FechaRegistro,
                CostoTotal = r.CostoTotal,
                TotalCargos = r.CargosAdicionales.Sum(c => c.Monto)
            }).ToList();

            var cols = new Dictionary<string, Func<dynamic, object>>
              {
                  { "FechaRegistro", x => x.Fecha },
                  { "CostoTotal", x => x.CostoTotal },
                  { "TotalCargosAdicionales", x => x.TotalCargos }
              };
            fileContent = ExcelExportService.ExportToExcel(lista, cols, "Ingresos");
        }
        else
        {
            var query = _context.Reservas.AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
                query = query.Where(r => r.FechaInicio >= startDate && r.FechaFin <= endDate);

            var lista = query.Select(r => new
            {
                r.ReservaID,
                r.Vehiculo.Placa,
                Usuario = r.Usuario != null ? r.Usuario.NombreCompleto : r.ClienteReserva.NombreCompleto,
                r.FechaInicio,
                r.FechaFin,
                r.Estado
            }).ToList();

            var cols = new Dictionary<string, Func<dynamic, object>>
              {
                  { "ReservaID", x => x.ReservaID },
                  { "Placa", x => x.Placa },
                  { "Usuario/Cliente", x => x.Usuario },
                  { "FechaInicio", x => x.FechaInicio },
                  { "FechaFin", x => x.FechaFin },
                  { "Estado", x => x.Estado }
              };
            fileContent = ExcelExportService.ExportToExcel(lista, cols, "Alquileres");
        }

        return File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }
}


