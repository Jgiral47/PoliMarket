namespace PoliMarket.Core.Entities
{
    public class Cliente : Persona
    {
        public int CodigoCliente { get; set; }

        // Relaciones
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        // Método reutilizable del componente
        public List<OrdenEntrega> ConsultarHistorialOrdenes()
        {
            // Implementación placeholder - será completada con el repositorio
            return new List<OrdenEntrega>();
        }
    }
}