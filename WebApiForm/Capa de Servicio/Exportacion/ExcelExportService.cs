using ClosedXML.Excel;
using A = DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using WebApiForm.DTO__Data_Transfer_Object_;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml;

namespace WebApiForm.Capa_de_Servicio.Exportacion
{
    public class ExcelExportService
    {
        public byte[] GenerateExcelReport(List<ReportRespuestas_Dto> report)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Reporte");

            // Agregar encabezados
            worksheet.Cell(1, 1).Value = "ID Usuario";
            worksheet.Cell(1, 2).Value = "Nombre y Apellido";
            worksheet.Cell(1, 3).Value = "Usuario";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "ID Formulario";
            worksheet.Cell(1, 6).Value = "Fecha Inicio Encuesta";
            worksheet.Cell(1, 7).Value = "Hora Inicio Encuesta";
            worksheet.Cell(1, 8).Value = "Nombre Línea";
            worksheet.Cell(1, 9).Value = "Nombre Estación";
            worksheet.Cell(1, 10).Value = "ID Sección";
            worksheet.Cell(1, 11).Value = "Número Pregunta";
            worksheet.Cell(1, 12).Value = "Pregunta";
            worksheet.Cell(1, 13).Value = "Código SubPregunta";
            worksheet.Cell(1, 14).Value = "SubPregunta";
            worksheet.Cell(1, 15).Value = "Número Encuestas";
            worksheet.Cell(1, 16).Value = "Tipo Respuesta";
            worksheet.Cell(1, 17).Value = "Hora Respondida";
            worksheet.Cell(1, 18).Value = "Respuestas";
            worksheet.Cell(1, 19).Value = "Comentarios";
            worksheet.Cell(1, 20).Value = "Justificación";

            // Seleccionar el rango de los encabezados
            var headerRange = worksheet.Range("A1:T1");

            // Aplicar formato en negrita y fondo gris claro a los encabezados
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.DarkGreen;

            // Agregar datos
            int row = 2;
            foreach (var item in report)
            {
                worksheet.Cell(row, 1).Value = item.rp_IdUsuarios;
                worksheet.Cell(row, 2).Value = item.rp_NombreApellido;
                worksheet.Cell(row, 3).Value = item.rp_Usuarios;
                worksheet.Cell(row, 4).Value = item.rp_Email;
                worksheet.Cell(row, 5).Value = item.rp_IdFormulario;
                worksheet.Cell(row, 6).Value = item.rp_FechaInicioEncuesta;
                worksheet.Cell(row, 7).Value = item.rp_HoraInicioEncuesta;
                worksheet.Cell(row, 8).Value = item.rp_NombreLinea;
                worksheet.Cell(row, 9).Value = item.rp_NombreEstacion;
                worksheet.Cell(row, 10).Value = item.rp_IdSesion;
                worksheet.Cell(row, 11).Value = item.rp_CodPreg;
                worksheet.Cell(row, 12).Value = item.rp_Pregunta;
                worksheet.Cell(row, 13).Value = item.rp_CodSubPreg;
                worksheet.Cell(row, 14).Value = item.rp_SubPregunta;
                worksheet.Cell(row, 15).Value = item.rp_NoEncuestas;
                worksheet.Cell(row, 16).Value = item.rp_TipoResp;
                worksheet.Cell(row, 17).Value = item.rp_HoraRespondida;
                worksheet.Cell(row, 18).Value = item.rp_Respuestas;
                worksheet.Cell(row, 19).Value = item.rp_Comentarios;
                worksheet.Cell(row, 20).Value = item.rp_Justificacion;
                row++;
            }

            // Definir el rango de la tabla incluyendo encabezados y datos
            var dataRange = worksheet.Range(1, 1, row - 1, 20);

            // Crear la tabla a partir del rango de datos
            var excelTable = dataRange.CreateTable();

            // Asignar un estilo a la tabla (opcional)
            excelTable.Theme = XLTableTheme.TableStyleDark11;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream.ToArray();
        }
    }
}
