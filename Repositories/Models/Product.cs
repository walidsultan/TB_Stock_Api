using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace TB_Stock.DAL.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Color { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Price { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Material { get; set; }
        public int Quantity { get; set; }
        public string InstagramRefId { get; set; }
        public string InstagramContentUrl { get; set; }


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
              .Property(t => t.DepartmentId)
              .HasColumnName("DepartmentId")
              .HasColumnType("int");

            profileConfig
                .Property(t => t.Category)
                .HasColumnName("Category")
                .HasColumnType("nvarchar");

            profileConfig
               .Property(t => t.Name)
               .HasColumnName("Name")
               .HasColumnType("nvarchar");


            profileConfig
               .Property(t => t.Code)
               .HasColumnName("Code")
               .HasColumnType("nvarchar");


            profileConfig
               .Property(t => t.Color)
               .HasColumnName("Color")
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

            profileConfig
               .Property(t => t.Size)
               .HasColumnName("Size")
               .HasColumnType("nvarchar");

            profileConfig
               .Property(t => t.Type)
               .HasColumnName("Type")
               .HasColumnType("nvarchar");

            profileConfig
               .Property(t => t.Material)
               .HasColumnName("Material")
               .HasColumnType("nvarchar");

            profileConfig
               .Property(t => t.Quantity)
               .HasColumnName("Quantity")
               .HasColumnType("int");

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
