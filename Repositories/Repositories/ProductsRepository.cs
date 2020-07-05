using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TB_Stock.DAL.Models;
using TBStock.DAL.Models;

namespace TBStock.DAL.Repositories
{
    public class ProductsRepository : IProductsRepository
    {

        public IEnumerable<Product> GetProductsByDepartmentId(int departmentId)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.Where(x => x.DepartmentId == departmentId).ToList();
            }
        }

        public IEnumerable<ProductDetail> GetProductDetailsByProductId(int productId)
        {
            using (var context = new TBStockDBContext())
            {
                return context.ProductDetails.Where(x => x.ProductId == productId).ToList();
            }
        }

        public void AddProducts(IEnumerable<Product> products)
        {
            using (var context = new TBStockDBContext())
            {
                context.Products.AddRange(products);

                context.SaveChanges();
            }
        }

        public int GetProductsCount()
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.Count();
            }
        }

        public void AddProductsDetails(IEnumerable<ProductDetail> productsDetails)
        {
            using (var context = new TBStockDBContext())
            {
                context.ProductDetails.AddRange(productsDetails);

                context.SaveChanges();
            }
        }

        public Product GetProductByProductId(int productId)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.FirstOrDefault(x => x.Id == productId);
            }
        }

        public Product GetProductByCode(string code)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.FirstOrDefault(x => x.Code == code);
            }
        }

        public IEnumerable<Product> GetRandomProducts(int count)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.OrderBy(x => Guid.NewGuid()).Take(count).ToList();
            }
        }

        public IEnumerable<Product> GetProductsByCategory(string category, ProductDepartment department, int skip, int take)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.Where(x => x.Category == category && x.DepartmentId==(int)department).OrderBy(x=>x.Id).Skip(skip).Take(take).ToList();
            }
        }

        public void DeleteAllProducts()
        {
            using (var context = new TBStockDBContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM [walidaly_TB_Stock].[walid].[Products]");
            }
        }

        public void DeleteAllProductsDetails()
        {
            using (var context = new TBStockDBContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE [walidaly_TB_Stock].[walid].[ProductDetails]");
            }
        }
    }
}
