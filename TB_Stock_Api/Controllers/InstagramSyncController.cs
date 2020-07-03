using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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

        private const string SIZE_IDENTIFIER = "size";
        private const string SIZES_IDENTIFIER = "sizes";
        private const string MEN_IDENTIFIER = "men";
        private const string WOMEN_IDENTIFIER = "ladies";
        private const string CATEGORY_IDENTIFIER = "category";
        private const string DEPARTMENT_IDENTIFIER = "department";
        private const string COLOR_IDENTIFIER = "color";
        private const string QUANTITY_IDENTIFIER = "qty";
        private const string PRICE_IDENTIFIER = "price";
        private const string TYPE_IDENTIFIER = "type";
        private const string MATERIAL_IDENTIFIER = "material";
        private const string GENDER_IDENTIFIER = "gender";
        private const string BRAND_IDENTIFIER = "brand";
        private const string BRANDS_IDENTIFIER = "brands";

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
                    ProductsList productsList = ConvertInstagramPostToProducts(posts);

                    //Delete all existing data
                    _ProductsRepository.DeleteAllProducts();
                    _ProductsRepository.DeleteAllProductsDetails();

                    //Add products to DB
                    _ProductsRepository.AddProducts(productsList.Products);
                    _ProductsRepository.AddProductsDetails(productsList.ProductsDetails);

                    ////Backup existing images
                    //FTPHandler.BackupDirectory(IMAGES_FOLDER_NAME, ftpUsername, ftpPassword);

                    ////Create images
                    //CreateNewImages(productsList, ftpUsername, ftpPassword);

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

        private void CreateNewImages(ProductsList productsList, string ftpUsername, string ftpPassword)
        {
            //Create New Images Folder
            FTPHandler.CreateNewDirectory(IMAGES_FOLDER_NAME, ftpUsername, ftpPassword);

            //Create Category folders
            foreach (var departmentId in Enum.GetValues(typeof(ProductDepartment)).Cast<int>())
            {
                FTPHandler.CreateNewDirectory(IMAGES_FOLDER_NAME + "/" + departmentId.ToString(), ftpUsername, ftpPassword);
            }

            //Upload images
            string tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEMP_DOWNLOAD_FOLDER_NAME);
            using (var client = new WebClient())
            {
                foreach (var product in productsList.Products)
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }

                    var productDetails = productsList.ProductsDetails.Where(x => x.ProductId == product.Id);

                    var productFolderPath = IMAGES_FOLDER_NAME + "/" + product.DepartmentId.ToString() + "/" + product.Code;
                    FTPHandler.CreateNewDirectory(productFolderPath, ftpUsername, ftpPassword);

                    Dictionary<string,string> imagePaths = new Dictionary<string, string>();
                    if (productDetails.Count() > 0)
                    {
                        foreach (var productDetail in productDetails) {
                            imagePaths.Add(productDetail.ImagePath, productDetail.InstagramContentUrl);
                        }
                    }
                    else
                    {
                        imagePaths.Add(product.ImagePath,product.InstagramContentUrl);
                    }

                    foreach (var kvp in imagePaths)
                    {
                        string localFilePath = Path.Combine(tempDirectory, kvp.Key);
                        client.DownloadFile(kvp.Value, localFilePath);

                        string ftpFileName = productFolderPath + "/" + kvp.Key;
                        FTPHandler.UploadFile(ftpFileName, localFilePath, ftpUsername, ftpPassword);
                    }
                }
            }

            //Delete temp folder
            Directory.Delete(tempDirectory, true);
        }

        private ProductsList ConvertInstagramPostToProducts(IEnumerable<InstagramPost> posts)
        {
            ProductsList productList = new ProductsList();
            productList.Products = new List<Product>();
            productList.ProductsDetails = new List<ProductDetail>();
            int productId = 1;
            foreach (var post in posts)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(post.Caption))
                    {
                        continue;
                    }

                    string[] items = post.Caption.Split('\n');
                    int startIndex = 2;


                    if (items[2].IndexOf(CATEGORY_IDENTIFIER, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        startIndex = 1;
                    }
                    else
                    {
                        startIndex = 0;
                    }

                    Dictionary<string, string> itemStore = new Dictionary<string, string>();

                    for (int i = startIndex; i < items.Length; i++)
                    {
                        var identifier = items[i].Replace(". ", "").Split(' ')[0].Trim();
                        itemStore.Add(identifier.ToLower(), Regex.Replace(items[i], identifier, "").Trim());
                    }

                    var name = itemStore.ContainsKey(BRAND_IDENTIFIER) ? itemStore[BRAND_IDENTIFIER] : null;

                    if (string.IsNullOrEmpty(name)) {
                        name = itemStore.ContainsKey(BRANDS_IDENTIFIER) ? itemStore[BRANDS_IDENTIFIER] : null;
                    }

                    var category = itemStore.ContainsKey(CATEGORY_IDENTIFIER) ? itemStore[CATEGORY_IDENTIFIER] : null;

                    var size = itemStore.ContainsKey(SIZES_IDENTIFIER) ? itemStore[SIZES_IDENTIFIER] : null;

                    if (string.IsNullOrEmpty(size))
                    {
                        size = itemStore.ContainsKey(SIZE_IDENTIFIER) ? itemStore[SIZE_IDENTIFIER] : null;
                    }

                    var type = itemStore.ContainsKey(TYPE_IDENTIFIER) ? itemStore[TYPE_IDENTIFIER] : null;

                    var color = itemStore.ContainsKey(COLOR_IDENTIFIER) ? itemStore[COLOR_IDENTIFIER] : null;

                    var material = itemStore.ContainsKey(MATERIAL_IDENTIFIER) ? itemStore[MATERIAL_IDENTIFIER] : null;

                    var department = itemStore.ContainsKey(DEPARTMENT_IDENTIFIER) ? itemStore[DEPARTMENT_IDENTIFIER] : null;

                    if (string.IsNullOrEmpty(department))
                    {
                        department = itemStore.ContainsKey(GENDER_IDENTIFIER) ? itemStore[GENDER_IDENTIFIER] : null;
                    }

                    department = department.Replace(" ", "");

                    var departmentId = (int)Enum.Parse(typeof(ProductDepartment), department, true);

                    var quantity = itemStore.ContainsKey(QUANTITY_IDENTIFIER) ? int.Parse(itemStore[QUANTITY_IDENTIFIER]) : 0;

                    var priceValue = itemStore.ContainsKey(PRICE_IDENTIFIER) ? itemStore[PRICE_IDENTIFIER] : null;

                    int spaceAfterPrice = priceValue.IndexOf(" ");

                    double price = double.TryParse(priceValue.Substring(0, spaceAfterPrice), out price) ? price : 0;

                    string imagePath = Path.GetFileName(new Uri(post.Media_Url).LocalPath);

                    var product = new Product()
                    {
                        Price = price,
                        Quantity = quantity,
                        Size = size,
                        CreatedDate = DateTime.Now,
                        Code = post.Id,
                        Material = material,
                        DepartmentId = departmentId,
                        Color = color,
                        Category = category,
                        Name = name,
                        ImagePath = imagePath,
                        InstagramRefId = post.Id,
                        InstagramContentUrl = post.Media_Url,
                        Id = productId
                    };
                    productList.Products.Add(product);


                    //Add product details
                    if (post.Children != null)
                    {
                        foreach (var childPost in post.Children.Data)
                        {
                            string childImagePath = Path.GetFileName(new Uri(childPost.Media_Url).LocalPath);

                            productList.ProductsDetails.Add(new ProductDetail()
                            {
                                ImagePath = childImagePath,
                                InstagramContentUrl = childPost.Media_Url,
                                InstagramRefId = childPost.Id,
                                ProductId = product.Id
                            });
                        }
                    }

                    productId++;
                }
                catch
                {
                    continue;
                }
            }

            return productList;
        }
    }
}
