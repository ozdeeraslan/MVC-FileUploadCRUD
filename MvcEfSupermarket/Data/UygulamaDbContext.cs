using Microsoft.EntityFrameworkCore;

namespace MvcEfSupermarket.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
        {

        }
        public DbSet<Urun> Urunler { get; set; }

        
    }
}
