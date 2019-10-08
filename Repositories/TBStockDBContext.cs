using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_Stock.DAL.Models;

namespace TBStock.DAL
{
    public class TBStockDBContext : DbContext
    {
        public TBStockDBContext() : base("TBStockDBConnectionString")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("walid");

            Product.SetEntityConfiguration(modelBuilder);
            ProductDetail.SetEntityConfiguration(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }

    }
}
