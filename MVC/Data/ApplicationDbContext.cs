using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
        public class ApplicationDbContext : DbContext
        {
                public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
                { }
                public DbSet<Book> Book { get; set; }
                public DbSet<SinhVien> SinhVien { get; set; }
        }
}