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
    [RoutePrefix("api/Categories")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CatgeoriesController : ApiController
    {
        private ProductsRepository _ProductsRepository;

        private static IEnumerable<DepartmentCategory> _UniqueCategories;

        public CatgeoriesController(ProductsRepository productsRepository)
        {
            _ProductsRepository = productsRepository;
        }

        [HttpGet]
        [Route("All")]
        public IEnumerable<DepartmentCategory> GetAllCategories()
        {
            if (_UniqueCategories == null)
            {
                _UniqueCategories = _ProductsRepository.GetUniqueCategories();
                return _UniqueCategories;
            }
            else
            {
                return _UniqueCategories;
            }
        }

    }
}
