using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class ProveedorService : BaseService<Proveedor>, IProveedorService
    {
        private readonly IGenericRepository<ProveedorProducto> _proveedorProductoRepository;

        public ProveedorService(
            IGenericRepository<Proveedor> repository,
            IGenericRepository<ProveedorProducto> proveedorProductoRepository)
            : base(repository)
        {
            _proveedorProductoRepository = proveedorProductoRepository;
        }

        // RF7: Consultar qué productos suministra cada proveedor
        public async Task<List<Producto>> ListarProductosSuministradosAsync(int proveedorId)
        {
            var proveedorProductos = await _proveedorProductoRepository.GetAllAsync(
                filter: pp => pp.IdProveedor == proveedorId && pp.Activo,
                includeProperties: "Producto"
            );

            return proveedorProductos.Select(pp => pp.Producto).ToList();
        }

        public async Task<string> ObtenerInfoSuministroAsync(int proveedorId, int productoId)
        {
            var proveedorProductos = await _proveedorProductoRepository.GetAllAsync(
                filter: pp => pp.IdProveedor == proveedorId && pp.IdProducto == productoId,
                includeProperties: "Proveedor,Producto"
            );

            var proveedorProducto = proveedorProductos.FirstOrDefault();
            return proveedorProducto?.ObtenerInfoSuministro() ?? "No encontrado";
        }
    }
}