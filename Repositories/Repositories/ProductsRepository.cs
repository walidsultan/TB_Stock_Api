using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TB_Stock.DAL.Models;

namespace TBStock.DAL.Repositories
{
    public class ProductsRepository: IProductsRepository
    {

        public IEnumerable<Product> GetProductsByCategoryId(int categoryId)
        {
            using (var context = new TBStockDBContext())
            {
                return context.Products.Where(x => x.CategoryId == categoryId).ToList();
            }
        }
    }
}
