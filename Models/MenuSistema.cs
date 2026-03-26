using System.ComponentModel.DataAnnotations;

namespace ProyectoCorporativoMvc.Models;

public class MenuSistema
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    [Display(Name = "Menú")]
    public string StrNombreMenu { get; set; } = string.Empty;

    public int Orden { get; set; }
    public ICollection<MenuModulo> MenuModulos { get; set; } = new List<MenuModulo>();
}
