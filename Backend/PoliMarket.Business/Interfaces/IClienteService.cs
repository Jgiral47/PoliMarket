using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IClienteService : IBaseService<Cliente>
    {
        // RF4: Consultar historial de órdenes del cliente
        Task<List<OrdenEntrega>> ConsultarHistorialOrdenesAsync(int clienteId);
        Task<List<Producto>> ListarProductosDisponiblesAsync();
    }
}