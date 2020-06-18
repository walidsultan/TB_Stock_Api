using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TB_Stock.Api.ApiHandler;
using TB_Stock.Api.FTP;
using TB_Stock.DAL.Models;
using TBStock.DAL.Models;
using TBStock.DAL.Repositories;

namespace TB_Stock.Api.Controllers
{
    [RoutePrefix("api/InstagramSync")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InstagramSyncController : ApiController
    {
        private const string INSTAGRAM_PAGE_URL = "https://www.instagram.com/original_brands_stock/";
        private const string IMAGES_FOLDER_NAME = "TBStock_Images";
        private const string TEMP_DOWNLOAD_FOLDER_NAME = "Temp";

        private static bool Is_Instagram_Reload_Running = false;

        private IInstagramGraphApi _IInstagramGraphApi;

        private IProductsRepository _ProductsRepository;

        private const string PRICE_IDENTIFIER = "price";
        private const string SIZE_IDENTIFIER = "size";
        private const string SIZES_IDENTIFIER = "sizes";
        private const string MEN_IDENTIFIER = "men";
        private const string WOMEN_IDENTIFIER = "ladies";

        public InstagramSyncController(IInstagramGraphApi instagramGraphApi, IProductsRepository productsRepository)
        {
            _IInstagramGraphApi = instagramGraphApi;
            _ProductsRepository = productsRepository;
        }

        [HttpPost]
        [Route("ReLoadAll")]
        public async Task<string> ReLoadAll(string accessToken, string ftpUsername, string ftpPassword)
        {
            try
            {
                if (!Is_Instagram_Reload_Running)
                {
                    Is_Instagram_Reload_Running = true;


                    //Get all instagram posts
                    var posts = await _IInstagramGraphApi.GetInstagramPosts(accessToken);

                    //Convert instagram posts to TB products
                    IEnumerable<Product> products = ConvertInstagramPostToProducts(posts);

                    //Delete all existing data
                    _ProductsRepository.DeleteAllProducts();

                    //Add products to DB
                    _ProductsRepository.AddProducts(products);

                    //Backup existing images
                    FTPHandler.BackupDirectory(IMAGES_FOLDER_NAME, ftpUsername, ftpPassword);

                    //Create images
                    CreateNewImages(products, ftpUsername, ftpPassword);

                    return "Done";
                }
                else
                {
                    return "Already_Running";
                }
            }
            finally
            {
                Is_Instagram_Reload_Running = false;
            }

        }

        private void CreateNewImages(IEnumerable<Product> products, string ftpUsername, string ftpPassword)
        {
            //Create New Images Folder
            FTPHandler.CreateNewDirectory(IMAGES_FOLDER_NAME, ftpUsername, ftpPassword);

            //Create Category folders
            foreach (var categoryId in Enum.GetValues(typeof(ProductCategory)).Cast<int>())
            {
                FTPHandler.CreateNewDirectory(IMAGES_FOLDER_NAME + "/" + categoryId.ToString(), ftpUsername, ftpPassword);
            }

            //Upload images
            string tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMP_DOWNLOAD_FOLDER_NAME);
            using (var client = new WebClient())
            {
                foreach (var product in products)
                {
                    if (!Directory.Exists(tempDirectory)) {
                        Directory.CreateDirectory(tempDirectory);
                    }
                    string localFilePath = Path.Combine(tempDirectory, product.ImagePath);
                    client.DownloadFile(product.InstagramContentUrl, localFilePath);

                    var productFolderPath = IMAGES_FOLDER_NAME + "/" + product.CategoryId.ToString() + "/" + product.Code;
                    FTPHandler.CreateNewDirectory(productFolderPath, ftpUsername, ftpPassword);

                    string ftpFileName = productFolderPath + "/" + product.ImagePath;
                    FTPHandler.UploadFile(ftpFileName, localFilePath, ftpUsername, ftpPassword);
                }
            }

            //Delete temp folder
            Directory.Delete(tempDirectory,true);
        }

        private IEnumerable<Product> ConvertInstagramPostToProducts(IEnumerable<InstagramPost> posts)
        {
            List<Product> products = new List<Product>();

            foreach (var post in posts)
            {
                if (string.IsNullOrWhiteSpace(post.Caption))
                {
                    continue;
                }

                int sizeIndex = post.Caption.IndexOf(SIZES_IDENTIFIER, 0, StringComparison.OrdinalIgnoreCase);
                if (sizeIndex < 0)
                {
                    sizeIndex = post.Caption.IndexOf(SIZE_IDENTIFIER, 0, StringComparison.OrdinalIgnoreCase);
                }

                int priceIndex = post.Caption.IndexOf(PRICE_IDENTIFIER, sizeIndex < 0 ? 0 : sizeIndex, StringComparison.OrdinalIgnoreCase);

                if (priceIndex < 0)
                {
                    continue;
                }

                string size = null;
                if (sizeIndex >= 0)
                {
                    int spaceAfterSize = post.Caption.IndexOf(" ", sizeIndex);
                    size = post.Caption.Substring(spaceAfterSize, priceIndex - spaceAfterSize);
                }

                int spaceAfterPrice = post.Caption.IndexOf(" ", priceIndex + PRICE_IDENTIFIER.Length + 1);

                int priceLength = spaceAfterPrice > 0 ? spaceAfterPrice - priceIndex - PRICE_IDENTIFIER.Length : post.Caption.Length - priceIndex - PRICE_IDENTIFIER.Length;

                double price = double.TryParse(post.Caption.Substring(priceIndex + PRICE_IDENTIFIER.Length, priceLength), out price) ? price : 0;
                var name = post.Caption.Substring(0, sizeIndex < 0 ? priceIndex : sizeIndex);

                var category = ProductCategory.Accessories;
                if (name.IndexOf(MEN_IDENTIFIER, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    category = ProductCategory.Men;
                }
                else if (name.IndexOf(WOMEN_IDENTIFIER, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    category = ProductCategory.Women;
                }

                string imagePath = Path.GetFileName(new Uri(post.Media_Url).LocalPath);

                products.Add(new Product()
                {
                    Price = price,
                    Size = size,
                    CreatedDate = DateTime.Now,
                    Code = post.Id,
                    Name = name,
                    ImagePath = imagePath,
                    InstagramRefId = post.Id,
                    CategoryId = (int)category,
                    InstagramContentUrl = post.Media_Url
                });
            }

            return products;
        }
    }
}
