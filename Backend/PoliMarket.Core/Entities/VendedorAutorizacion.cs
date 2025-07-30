namespace PoliMarket.Core.Entities
{
    public class VendedorAutorizacion : BaseEntity
    {
        public int IdVendedor { get; set; }
        public int IdAutorizacion { get; set; }
        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        public virtual Vendedor Vendedor { get; set; } = null!;
        public virtual Autorizacion Autorizacion { get; set; } = null!;

        // Método reutilizable del componente
        public string ObtenerEstadoAutorizacion()
        {
            return Autorizacion?.EsVigente() == true ? "AUTORIZADO" : "VENCIDO";
        }
    }
}