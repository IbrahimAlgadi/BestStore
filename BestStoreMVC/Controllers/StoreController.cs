using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BestStoreMVC.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 4;

        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {

            IQueryable<Product> query = context.Products;


            // search functionality
            if (search != null)
            {
                search = search.Trim();
                Console.WriteLine("Search Value: " + search);
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            // ordering
            // sort functionality
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                query = query.OrderByDescending(p => p.Id);
            }


            // pagination
            if (pageIndex < 1)
                pageIndex = 1;

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            // list records
            var products = query.ToList();

            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort,
            };

            return View(storeSearchModel);
        }


        public IActionResult Details(int id) 
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Store", "Index");
            }

            return View(product);
        }


    }
}
