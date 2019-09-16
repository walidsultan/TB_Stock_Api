using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TB_Stock.DAL.Models;
using TBStock.DAL.Repositories;

namespace TB_Stock.Api.Controllers
{
    [RoutePrefix("api/Products")]
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

      
    }
}
