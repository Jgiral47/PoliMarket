namespace PoliMarket.Business.DTOs
{
        public class DisponibilidadStockDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int StockDisponible { get; set; }
        public int CantidadSolicitada { get; set; }
        public bool Disponible { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}