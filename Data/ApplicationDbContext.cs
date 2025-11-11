using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using randevuappapi.Models;

namespace randevuappapi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Örnek tablo (DbSet)
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // Diğer tablolar da burada olacak:
        // public DbSet<Product> Products { get; set; }
        // public DbSet<Appointment> Appointments { get; set; }
    }
}
