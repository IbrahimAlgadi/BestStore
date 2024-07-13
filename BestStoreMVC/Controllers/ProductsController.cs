using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webEenv;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webEenv)
        {
            this.context = context;
            this.webEenv = webEenv;
        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {

            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // Save Image File
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile?.FileName);

            string imageFullPath = webEenv.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto?.ImageFile?.CopyTo(stream);
            }

            // save to database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFile = newFileName,
                CreatedAt = DateTime.Now,
            };

            // save object to the database
            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // save to database
            ProductDto productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            // Add more information to the view data
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFile"] = product.ImageFile;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");


            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {

            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                // Add more information to the view data
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFile"] = product.ImageFile;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // Update Image File
            if (productDto.ImageFile != null)
            {
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile?.FileName);

                string imageFullPath = webEenv.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto?.ImageFile?.CopyTo(stream);
                }

                // delete the old image
                string oldImageFilePath = webEenv.WebRootPath + "/products/" + product.ImageFile;
                System.IO.File.Delete(oldImageFilePath);

                product.ImageFile = newFileName;
            }

            // save to database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.CreatedAt = DateTime.Now;

            // save object to the database
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }
    }
}
