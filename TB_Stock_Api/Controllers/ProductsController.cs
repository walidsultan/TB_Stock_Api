using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TB_Stock.DAL.Models;
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
        [Route("Category/{categoryId}")]
        public IEnumerable<Product> GetByCategoryId(int categoryId)
        {
            return _ProductsRepository.GetProductsByCategoryId(categoryId);
        }


        [HttpGet]
        [Route("Product/{productId}")]
        public IEnumerable<ProductDetail> GetByProductId(int productId)
        {
            return _ProductsRepository.GetProductDetailsByProductId(productId);
        }


    }
}
