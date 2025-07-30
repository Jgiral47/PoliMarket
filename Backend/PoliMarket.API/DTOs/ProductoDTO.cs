using System.ComponentModel.DataAnnotations;

namespace PoliMarket.API.DTOs
{
    public class ProductoDTO : BaseDTO
    {
        public int IdProducto { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public double Precio { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        public int StockDisponible { get; set; }
    }
}