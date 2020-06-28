using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_Stock.DAL.Models;

namespace TBStock.DAL.Models
{
    public class ProductsList
    {
        public List<Product> Products {get;set;}
        public List<ProductDetail> ProductsDetails {get;set;}
    }
}
