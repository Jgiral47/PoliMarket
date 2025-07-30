using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IProveedorService : IBaseService<Proveedor>
    {
        // RF7: Consultar qué productos suministra cada proveedor
        Task<List<Producto>> ListarProductosSuministradosAsync(int proveedorId);
        Task<string> ObtenerInfoSuministroAsync(int proveedorId, int productoId);
    }
}