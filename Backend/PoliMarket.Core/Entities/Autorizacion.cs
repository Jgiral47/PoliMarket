using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    public class Autorizacion : BaseEntity
    {
        public int IdAutorizacion { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty;

        public DateTime FechaVigencia { get; set; }

        // Relaciones
        public virtual ICollection<VendedorAutorizacion> VendedoresAutorizacion { get; set; } = new List<VendedorAutorizacion>();

        // Método reutilizable del componente
        public bool EsVigente()
        {
            return FechaVigencia > DateTime.UtcNow;
        }
    }
}