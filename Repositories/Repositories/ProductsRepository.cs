using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TB_Stock.DAL.Models;

namespace TBStock.DAL.Repositories
{
    public class ProductsRepository : IProductsRepository
    {

        public IEnumerable<Product> GetProductsByCategoryId(int categoryId)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.Where(x => x.CategoryId == categoryId).ToList();
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
