using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad Vendedor que hereda de Persona según diagrama UML
    /// RF1: Autorizar vendedores para operar dentro del sistema
    /// </summary>
    public class Vendedor : Persona
    {
        [Required]
        public int IdVendedor { get; set; }

        public bool EstaAutorizado { get; set; } = false;

        public DateTime? FechaAutorizacion { get; set; }

        // Relaciones
        public virtual ICollection<Autorizacion> Autorizaciones { get; set; } = new List<Autorizacion>();
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        // Método de negocio para RF1
        public bool PuedeVender()
        {
            return EstaAutorizado && Activo;
        }
    }
}