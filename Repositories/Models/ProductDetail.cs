using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace TB_Stock.DAL.Models
{
    public class ProductDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public string InstagramRefId { get; set; }
        public string InstagramContentUrl { get; set; }
        public static void SetEntityConfiguration(DbModelBuilder modelBuilder)
        {
            EntityTypeConfiguration<ProductDetail> profileConfig = modelBuilder.Entity<ProductDetail>();

            // Entity to Table mapping
            profileConfig.ToTable("ProductDetails");

            // Entity property to column name mapping
            #region - Column Mappings -
            profileConfig
                    .Property(t => t.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int");

            profileConfig
                .Property(t => t.ProductId)
                .HasColumnName("ProductId")
                .HasColumnType("int");

            profileConfig
               .Property(t => t.ImagePath)
               .HasColumnName("ImagePath")
               .HasColumnType("nvarchar");

            profileConfig
               .Property(t => t.InstagramRefId)
               .HasColumnName("InstagramRefId")
               .HasColumnType("nvarchar");

            profileConfig
               .Property(t => t.InstagramContentUrl)
               .HasColumnName("InstagramContentUrl")
               .HasColumnType("nvarchar");
            #endregion

            // Set Primary key
            profileConfig.HasKey(tz => new { tz.Id });
        }
    }
}
