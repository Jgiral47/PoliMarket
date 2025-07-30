using System.ComponentModel.DataAnnotations;

namespace PoliMarket.API.DTOs
{
    public abstract class PersonaDTO : BaseDTO
    {
        public int IdPersona { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; } = string.Empty;
    }
}