using System.ComponentModel.DataAnnotations;

namespace P2_AP1_JamesDePena.Models;

public class Pedidos
{
    [Key]
    public int PedidoId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZÑñáéíóúÁÉÍÓÚ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
    public string NombreCliente { get; set; } = string.Empty;

    [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0.")]
    public decimal Total { get; set; }
    public ICollection<PedidosDetalle> PedidosDetalles { get; set; } = new List<PedidosDetalle>();
}
