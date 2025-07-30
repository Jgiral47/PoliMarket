using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class ClienteService : BaseService<Cliente>, IClienteService
    {
        private readonly IGenericRepository<OrdenEntrega> _ordenEntregaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;

        public ClienteService(
            IGenericRepository<Cliente> repository,
            IGenericRepository<OrdenEntrega> ordenEntregaRepository,
            IGenericRepository<Producto> productoRepository)
            : base(repository)
        {
            _ordenEntregaRepository = ordenEntregaRepository;
            _productoRepository = productoRepository;
        }

        // RF4: Consultar historial de órdenes del cliente
        public async Task<List<OrdenEntrega>> ConsultarHistorialOrdenesAsync(int clienteId)
        {
            var ordenes = await _ordenEntregaRepository.GetAllAsync(
                filter: o => o.Venta.IdCliente == clienteId,
                orderBy: q => q.OrderByDescending(o => o.FechaEntrega),
                includeProperties: "Venta"
            );

            return ordenes.ToList();
        }

        public async Task<List<Producto>> ListarProductosDisponiblesAsync()
        {
            var productos = await _productoRepository.GetAllAsync(
                filter: p => p.Activo && p.Stock!.CantidadDisponible > 0,
                includeProperties: "Stock"
            );

            return productos.ToList();
        }
    }
}