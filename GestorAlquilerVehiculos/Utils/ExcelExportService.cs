using ClosedXML.Excel;

namespace GestorAlquilerVehiculos.Utils
{
    public class ExcelExportService
    {
        public static byte[] ExportToExcel<T>(
            IEnumerable<T> data,
            Dictionary<string, Func<T, object>> columns,
            string sheetName = "Reporte")
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(sheetName);

            // Encabezados  
            int colIndex = 1;
            foreach (var header in columns.Keys)
            {
                ws.Cell(1, colIndex).Value = header;
                colIndex++;
            }

            // Datos  
            int rowIndex = 2;
            foreach (var item in data)
            {
                colIndex = 1;
                foreach (var selector in columns.Values)
                {
                    // Convertir explícitamente el valor a XLCellValue  
                    ws.Cell(rowIndex, colIndex).Value = XLCellValue.FromObject(selector(item));
                    colIndex++;
                }
                rowIndex++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }

}
