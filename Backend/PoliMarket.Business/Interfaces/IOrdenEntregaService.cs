using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IOrdenEntregaService : IBaseService<OrdenEntrega>
    {
        // RF8: Registrar entregas realizadas y actualizar el estado de las órdenes
        Task<bool> RegistrarEntregaAsync(int ordenId, string nuevoEstado);
        Task<string> GenerarResumenAsync(int ordenId);

        // RF9: Ver transiciones de estado de una orden (historial de cambios)
        Task<List<HistoricoOrdenEntrega>> VerTransicionesAsync(int ordenId);
    }
}