using Microsoft.EntityFrameworkCore;

namespace ProyectoCorporativoMvc.ViewModels;

public class ResultadoPaginado<T>
{
    public IReadOnlyList<T> Items { get; private set; } = Array.Empty<T>();
    public int PaginaActual { get; private set; }
    public int TotalPaginas { get; private set; }
    public int TotalRegistros { get; private set; }
    public int TamanoPagina { get; private set; }

    public bool HayPaginaAnterior => PaginaActual > 1;
    public bool HayPaginaSiguiente => PaginaActual < TotalPaginas;

    public static async Task<ResultadoPaginado<T>> CrearAsync(IQueryable<T> consulta, int pagina, int tamanoPagina)
    {
        pagina = pagina < 1 ? 1 : pagina;
        var total = await consulta.CountAsync();
        var totalPaginas = (int)Math.Ceiling(total / (double)tamanoPagina);
        var items = await consulta.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToListAsync();

        return new ResultadoPaginado<T>
        {
            Items = items,
            PaginaActual = pagina,
            TotalPaginas = totalPaginas == 0 ? 1 : totalPaginas,
            TotalRegistros = total,
            TamanoPagina = tamanoPagina
        };
    }
}
