namespace SMART_ERP.Models;

public class CompanyConnection
{
    public int Id { get; set; }

    // Nombre visible de la empresa
    public string CompanyName { get; set; } = string.Empty;

    // LOCAL, LAN, REMOTA
    public string ConnectionType { get; set; } = "LOCAL";

    // Servidor o IP
    public string Server { get; set; } = "127.0.0.1";

    // Puerto MariaDB
    public int Port { get; set; } = 3307;

    // Base de datos exclusiva de esta empresa
    public string DatabaseName { get; set; } = string.Empty;

    // Usuario de conexión
    public string Username { get; set; } = string.Empty;

    // Contraseña
    public string Password { get; set; } = string.Empty;

    // Empresa disponible para seleccionar
    public bool IsActive { get; set; } = true;

    // Fecha de creación de la configuración
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Última conexión exitosa
    public DateTime? LastConnectionAt { get; set; }
}
