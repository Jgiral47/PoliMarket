using System.ComponentModel.DataAnnotations;

namespace PoliMarket.API.DTOs.Requests
{
    public class RegistrarVentaRequest
    {
        [Required]
        public int IdCliente { get; set; }

        [Required]
        public List<ProductoVentaRequest> Productos { get; set; } = new();
    }

    public class ProductoVentaRequest
    {
        [Required]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }

    public class AutorizarVendedorRequest
    {
        [Required]
        public int IdAutorizacion { get; set; }
    }
}