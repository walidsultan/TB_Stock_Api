using System.Collections.Generic;
using TB_Stock.DAL.Models;
using TBStock.DAL.Models;

namespace TBStock.DAL.Repositories
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetProductsByDepartmentId(int categoryId);
        IEnumerable<ProductDetail> GetProductDetailsByProductId(int productId);
        void DeleteAllProducts();
        void DeleteAllProductsDetails();
        void AddProducts(IEnumerable<Product> products);
        void AddProductsDetails(IEnumerable<ProductDetail> productsDetails);
        IEnumerable<Product> GetProductsByCategory(string category, ProductDepartment department, int skip, int take);
        Product GetProductByCode(string code);
        int GetProductsCount();
    }
}