using Microsoft.Extensions.Logging;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.DTOs;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;
using PoliMarket.DataAccess.Context;

namespace PoliMarket.Business.Services
{
    /// <summary>
    /// ComponenteVendedor - Implementa RF1: Autorizar vendedores para operar dentro del sistema
    /// Corresponde a ComponenteVendedor y ComponenteAutorizacion de la tabla de componentes
    /// </summary>
    public class ComponenteVendedor : IComponenteVendedor
    {
        private readonly IGenericRepository<Vendedor> _vendedorRepository;
        private readonly IGenericRepository<Cliente> _clienteRepository;
        private readonly IGenericRepository<Autorizacion> _autorizacionRepository;
        private readonly ILogger<ComponenteVendedor> _logger;

        public ComponenteVendedor(
            IGenericRepository<Vendedor> vendedorRepository,
            IGenericRepository<Cliente> clienteRepository,
            IGenericRepository<Autorizacion> autorizacionRepository,
            ILogger<ComponenteVendedor> logger)
        {
            _vendedorRepository = vendedorRepository;
            _clienteRepository = clienteRepository;
            _autorizacionRepository = autorizacionRepository;
            _logger = logger;
        }

        /// <summary>
        /// RF1: Autorizar vendedores para operar dentro del sistema
        /// Funcionalidad: autorizarVendedor(vendedorId, autorizacionId)
        /// </summary>
        public async Task<bool> AutorizarVendedorAsync(int vendedorId, int autorizacionId)
        {
            try
            {
                _logger.LogInformation("Iniciando autorización del vendedor {VendedorId}", vendedorId);

                // Buscar vendedor por ID
                var vendedor = await _vendedorRepository.Get(vendedorId);
                if (vendedor == null)
                {
                    _logger.LogWarning("Vendedor con ID {VendedorId} no encontrado", vendedorId);
                    return false;
                }

                // Verificar si ya está autorizado
                if (vendedor.EstaAutorizado)
                {
                    _logger.LogInformation("Vendedor {VendedorId} ya está autorizado", vendedorId);
                    return true;
                }

                // Actualizar estado del vendedor
                vendedor.EstaAutorizado = true;
                vendedor.FechaAutorizacion = DateTime.UtcNow;
                await _vendedorRepository.Update(vendedor);

                // Crear registro de autorización
                var autorizacion = new Autorizacion
                {
                    IdAutorizacion = autorizacionId,
                    IdVendedor = vendedorId,
                    Tipo = "Venta",
                    FechaVigencia = DateTime.UtcNow.AddYears(1),
                    EsVigente = true,
                    FechaCreacion = DateTime.UtcNow
                };

                await _autorizacionRepository.Add(autorizacion);

                _logger.LogInformation("Vendedor {VendedorId} autorizado exitosamente", vendedorId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autorizar vendedor {VendedorId}", vendedorId);
                return false;
            }
        }

        /// <summary>
        /// Funcionalidad: estaAutorizado()
        /// Verifica si un vendedor está autorizado para operar
        /// </summary>
        public async Task<bool> EstaAutorizadoAsync(int vendedorId)
        {
            try
            {
                var vendedor = await _vendedorRepository.Get(vendedorId);
                if (vendedor == null) return false;

                // Verificar autorización del vendedor
                if (!vendedor.EstaAutorizado || !vendedor.Activo) return false;

                // Verificar autorizaciones vigentes
                var autorizaciones = await _autorizacionRepository.GetAllAsync(
                    filter: a => a.IdVendedor == vendedorId && a.EsVigente,
                    orderBy: q => q.OrderByDescending(a => a.FechaCreacion)
                );

                var autorizacionVigente = autorizaciones.FirstOrDefault();
                return autorizacionVigente?.ObtenerEstadoAutorizacion() ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar autorización del vendedor {VendedorId}", vendedorId);
                return false;
            }
        }

        /// <summary>
        /// Funcionalidad: obtenerEstadoAutorizacion()
        /// Obtiene el estado detallado de autorización de un vendedor
        /// </summary>
        public async Task<EstadoAutorizacionDto> ObtenerEstadoAutorizacionAsync(int vendedorId)
        {
            try
            {
                var vendedor = await _vendedorRepository.Get(vendedorId);
                if (vendedor == null)
                {
                    return new EstadoAutorizacionDto
                    {
                        VendedorId = vendedorId,
                        Estado = "No encontrado",
                        EstaAutorizado = false,
                        Autorizado = false
                    };
                }

                var estaAutorizado = await EstaAutorizadoAsync(vendedorId);

                return new EstadoAutorizacionDto
                {
                    VendedorId = vendedorId,
                    NombreVendedor = vendedor.NombreCompleto,
                    EstaAutorizado = vendedor.EstaAutorizado,
                    FechaAutorizacion = vendedor.FechaAutorizacion,
                    Estado = estaAutorizado ? "Autorizado" : "No autorizado",
                    Autorizado = estaAutorizado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado de autorización del vendedor {VendedorId}", vendedorId);
                return new EstadoAutorizacionDto
                {
                    VendedorId = vendedorId,
                    Estado = "Error",
                    EstaAutorizado = false,
                    Autorizado = false
                };
            }
        }

        /// <summary>
        /// Funcionalidad: listarVendedores()
        /// Lista todos los vendedores del sistema
        /// </summary>
        public async Task<IEnumerable<Vendedor>> ListarVendedoresAsync()
        {
            try
            {
                return await _vendedorRepository.GetAllAsync(
                    orderBy: q => q.OrderBy(v => v.Nombre).ThenBy(v => v.Apellido)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar vendedores");
                return new List<Vendedor>();
            }
        }

        /// <summary>
        /// Lista vendedores autorizados para operar
        /// </summary>
        public async Task<IEnumerable<Vendedor>> ListarVendedoresAutorizadosAsync()
        {
            try
            {
                return await _vendedorRepository.GetAllAsync(
                    filter: v => v.EstaAutorizado && v.Activo,
                    orderBy: q => q.OrderBy(v => v.Nombre)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar vendedores autorizados");
                return new List<Vendedor>();
            }
        }

        /// <summary>
        /// Lista vendedores pendientes de autorización
        /// </summary>
        public async Task<IEnumerable<Vendedor>> ListarVendedoresPendientesAsync()
        {
            try
            {
                return await _vendedorRepository.GetAllAsync(
                    filter: v => !v.EstaAutorizado && v.Activo,
                    orderBy: q => q.OrderBy(v => v.FechaCreacion)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar vendedores pendientes");
                return new List<Vendedor>();
            }
        }

        /// <summary>
        /// Funcionalidad: listarClientes()
        /// Lista clientes disponibles para ventas
        /// </summary>
        public async Task<IEnumerable<Cliente>> ListarClientesAsync()
        {
            try
            {
                return await _clienteRepository.GetAllAsync(
                    filter: c => c.Activo,
                    orderBy: q => q.OrderBy(c => c.Nombre).ThenBy(c => c.Apellido)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar clientes");
                return new List<Cliente>();
            }
        }
    }
}