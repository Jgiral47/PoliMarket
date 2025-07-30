using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class OrdenEntregaService : BaseService<OrdenEntrega>, IOrdenEntregaService
    {
        private readonly IGenericRepository<HistoricoOrdenEntrega> _historicoRepository;

        public OrdenEntregaService(
            IGenericRepository<OrdenEntrega> repository,
            IGenericRepository<HistoricoOrdenEntrega> historicoRepository)
            : base(repository)
        {
            _historicoRepository = historicoRepository;
        }

        // RF8: Registrar entregas realizadas y actualizar el estado de las órdenes
        public async Task<bool> RegistrarEntregaAsync(int ordenId, string nuevoEstado)
        {
            try
            {
                var orden = await _repository.Get(ordenId);
                if (orden == null) return false;

                // Utilizar método del componente para actualizar estado
                orden.ActualizarEstado(nuevoEstado);
                await _repository.Update(orden);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerarResumenAsync(int ordenId)
        {
            var orden = await _repository.Get(ordenId);
            return orden?.GenerarResumen() ?? "Orden no encontrada";
        }

        // RF9: Ver transiciones de estado de una orden (historial de cambios)
        public async Task<List<HistoricoOrdenEntrega>> VerTransicionesAsync(int ordenId)
        {
            var historico = await _historicoRepository.GetAllAsync(
                filter: h => h.IdOrden == ordenId,
                orderBy: q => q.OrderBy(h => h.FechaCambio)
            );

            return historico.ToList();
        }
    }
}