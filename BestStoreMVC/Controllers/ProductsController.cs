using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Controllers
{
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webEenv;
        private readonly int pageSize = 5;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webEenv)
        {
            this.context = context;
            this.webEenv = webEenv;
        }
        public IActionResult Index(int pageIndex, string? search, string? orderBy, string? column)
        {
            IQueryable<Product> query = context.Products;
            // search
            if (search != null)
            {
                search = search.Trim();
                Console.WriteLine("Search Value: " + search);
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            // sort functionality
            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if (string.IsNullOrEmpty(column) || !validColumns.Contains(column))
            {
                column = "Id";
            }

            if (string.IsNullOrEmpty(orderBy) || !validOrderBy.Contains(orderBy))
            {
                column = "desc";
            }

            // ordering
            // query = query.OrderByDescending(p => p.Id);

            //if (column == "Name")
            //{
            //    if (orderBy == "asc")
            //    {
            //        query = query.OrderBy(p => p.Name);
            //    }
            //    else
            //    {
            //        query = query.OrderByDescending(p => p.Name);
            //    }
            //}

            switch (column)
            {
                case "Id":
                    {
                        if (orderBy == "asc")
                        {
                            query = query.OrderBy(p => p.Id);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Id);
                        }
                        break; 
                    }
                case "Name":
                    {
                        if (orderBy == "asc")
                        {
                            query = query.OrderBy(p => p.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Name);
                        }
                        break;
                    }
                case "Brand":
                    {
                        if (orderBy == "asc")
                        {
                            query = query.OrderBy(p => p.Brand);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Brand);
                        }
                        break;
                    }
                case "Category":
                    {
                        if (orderBy == "asc")
                        {
                            query = query.OrderBy(p => p.Category);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Category);
                        }
                        break;
                    }
                case "Price":
                    {
                        if (orderBy == "asc")
                        {
                            query = query.OrderBy(p => p.Price);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Price);
                        }
                        break;
                    }
                case "CreatedAt":
                    {
                        if (orderBy == "asc")
                        {
                            query = query.OrderBy(p => p.CreatedAt);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.CreatedAt);
                        }
                        break;
                    }
                default: break;
            }
            

            //if (orderBy == "asc")
            //{
            //    query.OrderBy(e => EF.Property<object>(e, column));
            //}
            //else
            //{
            //    query.OrderByDescending(e => EF.Property<object>(e, column));
            //}

            //query = orderBy == "asc" ?
            //    query.OrderBy(e => EF.Property<object>(e, column))
            //    : 
            //    query.OrderByDescending(e => EF.Property<object>(e, column));


            // pagination
            if (pageIndex < 0)
            {
                pageIndex = 1;
            } 

            decimal count = query.Count();
            int totalPages = (int) Math.Ceiling(count / pageSize);
            
            if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }
            var offsetValue = ((int)pageIndex - 1) * pageSize;

            query = query.Skip(offsetValue > 1 ? offsetValue : 0).Take(pageSize); 

            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            
            ViewData["Search"] = search ?? "";

            ViewData["OrderBy"] = orderBy;
            ViewData["Column"] = column;

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

        public IActionResult Delete(int id)
        {

            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // delete the product image
            string oldImageFilePath = webEenv.WebRootPath + "/products/" + product.ImageFile;
            System.IO.File.Delete(oldImageFilePath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }
    }
}
