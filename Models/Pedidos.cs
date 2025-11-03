using System.ComponentModel.DataAnnotations;

namespace P2_AP1_JamesDePena.Models;

public class Pedidos
{
    [Key]
    public int PedidoId { get; set; }
    public DateTime Fecha { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
