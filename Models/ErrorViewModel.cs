namespace ProyectoCorporativoMvc.Models;

public class ErrorViewModel
{
    public int StatusCode { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? RequestId { get; set; }
}
