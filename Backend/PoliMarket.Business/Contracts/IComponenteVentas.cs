
using PoliMarket.Core.Entities;
using PoliMarket.Business.DTOs;

namespace PoliMarket.Business.Contracts
{
    /// <summary>
    /// ComponenteVentas - RF2 y RF3
    /// RF2: Registrar ventas de productos a clientes
    /// RF3: Calcular totales, generar factura y agregar productos a una venta
    /// </summary>
    public interface IComponenteVentas
    {
        // RF2: Registrar venta
        Task<Venta> RegistrarVentaAsync(int idVendedor, int idCliente, List<ProductoVentaDto> productos);

        // RF3: Calcular total
        Task<decimal> CalcularTotalAsync(int ventaId);

        // RF3: Generar factura
        Task<FacturaDto> GenerarFacturaAsync(int ventaId);

        // RF3: Agregar producto a venta existente
        Task<bool> AgregarProductoAsync(int ventaId, int idProducto, int cantidad);

        // Métodos auxiliares
        Task<IEnumerable<Venta>> ObtenerVentasPorVendedorAsync(int vendedorId);
        Task<IEnumerable<Venta>> ObtenerVentasPorClienteAsync(int clienteId);
        Task<Venta?> ObtenerVentaCompletaAsync(int ventaId);
        Task<bool> CompletarVentaAsync(int ventaId);
        Task<bool> CancelarVentaAsync(int ventaId);
    }
}
