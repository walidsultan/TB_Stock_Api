using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TB_Stock.Api.ApiHandler;
using TBStock.DAL.Repositories;
using Unity;

namespace TB_Stock
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterType<IProductsRepository, ProductsRepository>();
            container.RegisterType<IInstagramGraphApi, InstagramGraphApi>();
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
