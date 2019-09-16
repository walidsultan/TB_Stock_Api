using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace TB_Stock.DAL.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Price { get; set; }


        public static void SetEntityConfiguration(DbModelBuilder modelBuilder)
        {
            EntityTypeConfiguration<Product> profileConfig = modelBuilder.Entity<Product>();

            // Entity to Table mapping
            profileConfig.ToTable("Products");

            // Entity property to column name mapping
            #region - Column Mappings -
            profileConfig
                    .Property(t => t.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int");

            profileConfig
                .Property(t => t.CategoryId)
                .HasColumnName("CategoryId")
                .HasColumnType("int");

            profileConfig
               .Property(t => t.Name)
               .HasColumnName("Name")
               .HasColumnType("nvarchar");


            profileConfig
               .Property(t => t.Code)
               .HasColumnName("Code")
               .HasColumnType("nvarchar");


            profileConfig
               .Property(t => t.ImagePath)
               .HasColumnName("ImagePath")
               .HasColumnType("nvarchar");


            profileConfig
               .Property(t => t.CreatedDate)
               .HasColumnName("CreatedDate")
               .HasColumnType("datetime");


            profileConfig
               .Property(t => t.Price)
               .HasColumnName("Price")
               .HasColumnType("float");

            #endregion

            // Set Primary key
            profileConfig.HasKey(tz => new { tz.Id });
        }
    }
}
