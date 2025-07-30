using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IVendedorService : IBaseService<Vendedor>
    {
        // RF1: Autorizar vendedores para operar dentro del sistema
        Task<bool> AutorizarVendedorAsync(int vendedorId, int autorizacionId);
        Task<bool> EstaAutorizadoAsync(int vendedorId);
        Task<List<Cliente>> ListarClientesAsync(int vendedorId);
    }
}