namespace PoliMarket.API.DTOs
{
    public class VentaDTO : BaseDTO
    {
        public int IdVenta { get; set; }
        public int IdVendedor { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public string Estado { get; set; } = string.Empty;
        public double Total { get; set; }
        public List<ProductoVentaDTO> Productos { get; set; } = new();
    }

    public class ProductoVentaDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Subtotal { get; set; }
    }
}