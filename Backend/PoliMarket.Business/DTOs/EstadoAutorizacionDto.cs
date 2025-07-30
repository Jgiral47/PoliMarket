namespace PoliMarket.Business.DTOs
{    public class EstadoAutorizacionDto
    {
        public int VendedorId { get; set; }
        public string NombreVendedor { get; set; } = string.Empty;
        public bool EstaAutorizado { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool Autorizado { get; set; }
    }

}