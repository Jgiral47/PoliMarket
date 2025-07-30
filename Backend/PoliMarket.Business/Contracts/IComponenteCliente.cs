
using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Contracts
{
    /// <summary>
    /// ComponenteCliente - Operaciones sobre clientes  
    /// RF4: Consultar historial de órdenes del cliente
    /// </summary>
    public interface IComponenteCliente
    {
        // RF4: Consultar historial
        Task<IEnumerable<Venta>> ConsultarHistorialOrdenesAsync(int clienteId);

        // Métodos auxiliares
        Task<Cliente?> ObtenerClientePorIdAsync(int clienteId);
        Task<Cliente?> ObtenerClientePorCodigoAsync(int codigoCliente);
        Task<IEnumerable<Cliente>> ListarClientesActivosAsync();
        Task<bool> TieneComprasActivasAsync(int clienteId);
    }
}
