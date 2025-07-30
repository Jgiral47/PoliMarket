namespace PoliMarket.Core.Entities
{
    public class Vendedor : Persona
    {
        public bool EstaAutorizado { get; set; } = false;

        // Relaciones
        public virtual ICollection<VendedorAutorizacion> Autorizaciones { get; set; } = new List<VendedorAutorizacion>();
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        // Métodos reutilizables del componente
        public bool EstaActivo()
        {
            return Activo && EstaAutorizado;
        }

        public List<Cliente> ListarClientes()
        {
            // Implementación placeholder - será completada con el repositorio
            return new List<Cliente>();
        }
    }
}