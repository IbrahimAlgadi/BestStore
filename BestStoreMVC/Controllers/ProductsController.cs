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
                productDto.ImageFile.CopyTo(stream);
            }

            return RedirectToAction("Index", "Products");
        }
    }
}
