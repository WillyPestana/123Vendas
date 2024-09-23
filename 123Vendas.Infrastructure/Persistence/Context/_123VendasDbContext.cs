using _123Vendas.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace _123Vendas.Infrastructure.Persistence.Context
{
    public partial class _123VendasDbContext : DbContext
    {
        public _123VendasDbContext()
        {
        }

        public _123VendasDbContext(DbContextOptions<_123VendasDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Venda> Vendas { get; set; }
        public virtual DbSet<ItemVenda> ItensVenda { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venda>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Venda>()
                .HasMany(v => v.Itens)
                .WithOne(i => i.Venda)
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade); ;

            modelBuilder.Entity<ItemVenda>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.ValorTotal)
                .HasComputedColumnSql("[Quantidade] * ([ValorUnitario] - [Desconto])");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Venda>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Entity.ValorTotal = entry.Entity.Itens
                        .Where(i => !i.Cancelado) // Ignora itens cancelados
                        .Sum(i => i.ValorTotal);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
