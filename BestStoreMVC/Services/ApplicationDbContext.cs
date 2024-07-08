
using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services
{
    public class ApplicationDbContext: DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            // Call it options and pass it to the base class
        }

        // Name of table in the database
        public DbSet<Product> Products { get; set; }

    }
}
