using Microsoft.EntityFrameworkCore;
using ProyectoForex.Domain.Entities;

namespace ProyectoForex.Infrastructure.Data;
public sealed class ForexDbContext : DbContext
{
    public ForexDbContext(DbContextOptions<ForexDbContext> opt) : base(opt) { }
    public DbSet<Signal> Signals => Set<Signal>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Signal>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Ticker).HasMaxLength(50);
            e.Property(x => x.Action).HasMaxLength(10);
        });
    }
}
