using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad base Persona según diagrama UML
    /// </summary>
    public abstract class Persona
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdPersona { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Identificacion { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        // Propiedades de solo lectura para funcionalidades comunes
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}