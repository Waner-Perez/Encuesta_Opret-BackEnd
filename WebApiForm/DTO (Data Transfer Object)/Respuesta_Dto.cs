namespace WebApiForm.DTO__Data_Transfer_Object_
{
    public class Respuesta_Dto
    {
        public required string IdUsuarios { get; set; }
        public int IdSesion { get; set; }
        public required string Respuesta { get; set; }
        public string? Comentarios { get; set; }
        public string? Justificacion { get; set; }
        public string? HoraResp { get; set; }
        public string? FechaResp { get; set; }
        public int FinalizarSesion { get; set; }
    }
}
