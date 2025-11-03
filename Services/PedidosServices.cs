using Microsoft.EntityFrameworkCore;
using P2_AP1_JamesDePena.DAL;
using P2_AP1_JamesDePena.Models;
using System.Linq.Expressions;

namespace P2_AP1_JamesDePena.Services;

public class PedidosServices(IDbContextFactory<Contexto> DbFactory)
{
    private async Task<bool> Existe(int pedidoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Pedidos.AnyAsync(p => p.PedidoId == pedidoId);
    }

    private async Task AfectarProductos(PedidosDetalle[] detalle, TipoOperacion tipoOperacion)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        foreach (var item in detalle)
        {
            var componente = await contexto.Componentes.FirstOrDefaultAsync(c => c.ComponenteId == item.ComponenteId);
            if (tipoOperacion == TipoOperacion.Suma)
            {
                componente.Existencia += item.Cantidad;
            }
            else
            {
                componente.Existencia -= item.Cantidad;
            }
        }

        await contexto.SaveChangesAsync();
    }
    private async Task<bool> Insertar(Pedidos pedidos)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Pedidos.Add(pedidos);
        await AfectarProductos(pedidos.PedidosDetalles.ToArray(), TipoOperacion.Suma);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Pedidos pedido)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var anterior = await contexto.Pedidos.Include(p => p.PedidosDetalles).FirstOrDefaultAsync(p => p.PedidoId == pedido.PedidoId);

        if (anterior == null)
        {
            return false;
        }

        await AfectarProductos(anterior.PedidosDetalles.ToArray(), TipoOperacion.Resta);
        await AfectarProductos(anterior.PedidosDetalles.ToArray(), TipoOperacion.Suma);

        contexto.PedidosDetalles.RemoveRange(anterior.PedidosDetalles);
        anterior.PedidosDetalles = pedido.PedidosDetalles;
        anterior.Fecha = pedido.Fecha;
        anterior.NombreCliente = pedido.NombreCliente;
        anterior.Total = pedido.Total;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Pedidos pedido)
    {
        if (!await Existe(pedido.PedidoId))
        {
            return await Insertar(pedido);
        }
        else
        {
            return await Modificar(pedido);
        }
    }

    public async Task<bool> Eliminar(int pedidoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var pedidoExistente = await contexto.Pedidos.Include(p => p.PedidoId).FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

        if (pedidoExistente == null)
        {
            return false;
        }

        await AfectarProductos(pedidoExistente.PedidosDetalles.ToArray(), TipoOperacion.Resta);
        contexto.PedidosDetalles.RemoveRange(pedidoExistente.PedidosDetalles);
        contexto.Pedidos.Remove(pedidoExistente);

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Pedidos?> Buscar(int pedidoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Pedidos.Include(p => p.PedidosDetalles).FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
    }

    public async Task<List<Pedidos>> Listar(Expression<Func<Pedidos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Pedidos.Where(criterio).Include(p => p.PedidosDetalles).AsNoTracking().ToListAsync();
    }

    public async Task<List<Componente>> ListarComponentes(Expression<Func<Componente, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Componentes.Where(criterio).AsNoTracking().ToListAsync();
    }
}

public enum TipoOperacion
{
    Suma = 1,
    Resta = 2
}
