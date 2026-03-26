namespace ProyectoCorporativoMvc.Models;

public class MenuModulo
{
    public int Id { get; set; }
    public int IdMenu { get; set; }
    public int IdModulo { get; set; }
    public MenuSistema? Menu { get; set; }
    public Modulo? Modulo { get; set; }
}
