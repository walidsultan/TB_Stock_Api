using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TB_Stock.DAL.Models;
using TBStock.DAL.Models;
using TBStock.DAL.Repositories;

namespace TB_Stock.Api.Controllers
{
    [RoutePrefix("api/Products")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductsController : ApiController
    {
        private ProductsRepository _ProductsRepository;

        public ProductsController(ProductsRepository productsRepository)
        {
            _ProductsRepository = productsRepository;
        }

        [HttpGet]
        [Route("Department/{departmentId}")]
        public IEnumerable<Product> GetByDepartmentId(int departmentId)
        {
            return _ProductsRepository.GetProductsByDepartmentId(departmentId);
        }


        [HttpGet]
        [Route("ProductDetail/{productId}")]
        public IEnumerable<ProductDetail> GetProductDetailsByProductId(int productId)
        {
            return _ProductsRepository.GetProductDetailsByProductId(productId);
        }


        [HttpGet]
        [Route("Product/{productId}")]
        public Product GetProductByProductId(int productId)
        {
            return _ProductsRepository.GetProductByProductId(productId);
        }

        [HttpGet]
        [Route("Random/Count/{Count}")]
        public IEnumerable<Product> GetRandomProducts(int count)
        {
            return _ProductsRepository.GetRandomProducts(count);
        }

        [HttpGet]
        [Route("Category/{Category}/Department/{Department}")]
        public IEnumerable<Product> GetProductsByCategory(string category, ProductDepartment department, [FromUri] int skip=0,[FromUri] int take=20)
        {
            return _ProductsRepository.GetProductsByCategory(category, department,skip, take);
        }
    }
}
