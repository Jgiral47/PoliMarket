
using PoliMarket.Core.Entities;
using PoliMarket.Business.DTOs;

namespace PoliMarket.Business.Contracts
{
    /// <summary>
    /// ComponenteVendedor - RF1
    /// RF1: Autorizar vendedores para operar dentro del sistema
    /// </summary>
    public interface IComponenteVendedor
    {
        // RF1: Autorizar vendedor
        Task<bool> AutorizarVendedorAsync(int vendedorId, int autorizacionId);

        // Métodos auxiliares de RF1
        Task<bool> EstaAutorizadoAsync(int vendedorId);
        Task<EstadoAutorizacionDto> ObtenerEstadoAutorizacionAsync(int vendedorId);
        Task<IEnumerable<Vendedor>> ListarVendedoresAsync();
        Task<IEnumerable<Vendedor>> ListarVendedoresAutorizadosAsync();
        Task<IEnumerable<Vendedor>> ListarVendedoresPendientesAsync();
        Task<IEnumerable<Cliente>> ListarClientesAsync();
    }
}
