using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class VendedorService : BaseService<Vendedor>, IVendedorService
    {
        private readonly IGenericRepository<VendedorAutorizacion> _autorizacionRepository;
        private readonly IGenericRepository<Cliente> _clienteRepository;

        public VendedorService(
            IGenericRepository<Vendedor> repository,
            IGenericRepository<VendedorAutorizacion> autorizacionRepository,
            IGenericRepository<Cliente> clienteRepository)
            : base(repository)
        {
            _autorizacionRepository = autorizacionRepository;
            _clienteRepository = clienteRepository;
        }

        // RF1: Autorizar vendedores para operar dentro del sistema
        public async Task<bool> AutorizarVendedorAsync(int vendedorId, int autorizacionId)
        {
            try
            {
                var vendedor = await _repository.Get(vendedorId);
                if (vendedor == null) return false;

                // Crear nueva autorización
                var nuevaAutorizacion = new VendedorAutorizacion
                {
                    IdVendedor = vendedorId,
                    IdAutorizacion = autorizacionId,
                    FechaAsignacion = DateTime.UtcNow
                };

                await _autorizacionRepository.Add(nuevaAutorizacion);

                // Actualizar estado del vendedor
                vendedor.EstaAutorizado = true;
                await _repository.Update(vendedor);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EstaAutorizadoAsync(int vendedorId)
        {
            var vendedor = await _repository.Get(vendedorId);
            return vendedor?.EstaAutorizado ?? false;
        }

        public async Task<List<Cliente>> ListarClientesAsync(int vendedorId)
        {
            // Implementación del componente: listarClientes()
            var vendedor = await _repository.Get(vendedorId);
            if (vendedor == null || !vendedor.EstaAutorizado)
                return new List<Cliente>();

            return await _clienteRepository.GetAll();
        }
    }
}