using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    public class HistoricoOrdenEntrega : BaseEntity
    {
        public int IdHistorial { get; set; }
        public int IdOrden { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string EstadoAnterior { get; set; } = string.Empty;

        [StringLength(50)]
        public string EstadoNuevo { get; set; } = string.Empty;

        // Relaciones
        public virtual OrdenEntrega Orden { get; set; } = null!;

        // Método reutilizable del componente
        public string VerTransicion()
        {
            return $"{FechaCambio:dd/MM/yyyy HH:mm} - {EstadoAnterior} → {EstadoNuevo}";
        }
    }
}