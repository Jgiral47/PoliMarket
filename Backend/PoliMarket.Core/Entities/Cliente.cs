using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad Cliente que hereda de Persona según diagrama UML
    /// RF4: Consultar historial de órdenes del cliente
    /// </summary>
    public class Cliente : Persona
    {
        [Required]
        public int CodigoCliente { get; set; }

        public DateTime FechaRegistro { get; set; } 

        // Relaciones según diagrama UML
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        // Método de negocio para RF4: Consultar historial de órdenes
        public List<Venta> ConsultarHistorialOrdenes()
        {
            return Ventas.Where(v => v.IdCliente == this.Id)
                        .OrderByDescending(v => v.FechaVenta)
                        .ToList();
        }

        public bool TieneComprasActivas()
        {
            return Ventas.Any(v => v.Estado != "Completada" && v.Estado != "Cancelada");
        }
    }
}