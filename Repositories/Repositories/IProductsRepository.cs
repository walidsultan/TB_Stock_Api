using System.Collections.Generic;
using TB_Stock.DAL.Models;

namespace TBStock.DAL.Repositories
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetProductsByCategoryId(int categoryId);
        IEnumerable<ProductDetail> GetProductDetailsByProductId(int productId);
        void DeleteAllProducts();
        void DeleteAllProductsDetails();
        void AddProducts(IEnumerable<Product> products);

    }
}