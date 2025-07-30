using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IVentaService : IBaseService<Venta>
    {
        // RF2: Registrar ventas de productos a clientes
        Task<Venta> RegistrarVentaAsync(int vendedorId, int clienteId, List<(int productoId, int cantidad)> productos);

        // RF3: Calcular totales, generar factura y agregar productos a una venta
        Task<double> CalcularTotalAsync(int ventaId);
        Task<string> GenerarFacturaAsync(int ventaId);
        Task<bool> AgregarProductoAsync(int ventaId, int productoId, int cantidad);
    }
}