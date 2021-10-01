using Example.API.DataAccess.Configuration;
using Example.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Example.API.DataAccess.Context
{
    public partial class XUnitTestDBContext : DbContext
    {
        public XUnitTestDBContext()
        {
        }

        public XUnitTestDBContext(DbContextOptions<XUnitTestDBContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
