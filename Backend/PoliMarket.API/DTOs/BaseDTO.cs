namespace PoliMarket.API.DTOs
{
    public abstract class BaseDTO
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}