using Microsoft.EntityFrameworkCore;
using P2_AP1_JamesDePena.Models;

namespace P2_AP1_JamesDePena.DAL;

public class Contexto : DbContext
{
    public DbSet<Registro> Registro { get; set; }

    public Contexto(DbContextOptions<Contexto> options) : base(options) { }
}