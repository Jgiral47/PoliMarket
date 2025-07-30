using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    public class OrdenEntrega : BaseEntity
    {
        public int IdOrden { get; set; }
        public int IdVenta { get; set; }
        public DateTime FechaEntrega { get; set; }

        [StringLength(50)]
        public string EstadoEntrega { get; set; } = "PROGRAMADA";

        // Relaciones
        public virtual Venta Venta { get; set; } = null!;
        public virtual ICollection<HistoricoOrdenEntrega> HistorialCambios { get; set; } = new List<HistoricoOrdenEntrega>();

        // Métodos reutilizables del componente
        public void ActualizarEstado(string nuevoEstado)
        {
            var estadoAnterior = EstadoEntrega;
            EstadoEntrega = nuevoEstado;

            // Registrar en el histórico
            HistorialCambios.Add(new HistoricoOrdenEntrega
            {
                IdHistorial = 0, // Se asignará automáticamente
                FechaCambio = DateTime.UtcNow,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = nuevoEstado,
                Orden = this
            });
        }

        public string GenerarResumen()
        {
            return $"Orden #{IdOrden} - Estado: {EstadoEntrega} - Fecha: {FechaEntrega:dd/MM/yyyy}";
        }
    }
}