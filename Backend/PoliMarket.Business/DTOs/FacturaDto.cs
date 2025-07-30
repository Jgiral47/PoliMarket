namespace PoliMarket.Business.DTOs
{
    public class FacturaDto
    {
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaFactura { get; set; }
        public int VentaId { get; set; }
        public decimal Total { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public List<DetalleFacturaDto> Detalles { get; set; } = new();
    }


}