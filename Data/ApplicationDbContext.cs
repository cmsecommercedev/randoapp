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

        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<WorkingHour> WorkingHours => Set<WorkingHour>();
        public DbSet<WorkingTimeSlot> WorkingTimeSlots => Set<WorkingTimeSlot>();
        public DbSet<WorkingTimeSlotException> WorkingTimeSlotExceptions => Set<WorkingTimeSlotException>();

        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<AppointmentReview> AppointmentReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🏢 Business → Category
            modelBuilder.Entity<Business>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 💼 Business → Employees
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Business)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // 💈 Business → Services
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Business)
                .WithMany(b => b.Services)
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Service.Price için hassasiyet ayarı
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // 🕐 Business → WorkingHours
            modelBuilder.Entity<WorkingHour>()
                .HasOne(w => w.Business)
                .WithMany(b => b.WorkingHours)
                .HasForeignKey(w => w.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📅 Appointment → Business
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Business)
                .WithMany(b => b.Appointments)
                .HasForeignKey(a => a.BusinessId)
                .OnDelete(DeleteBehavior.Restrict); // ❗ Cascade değil

            // 📅 Appointment → Employee
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Appointments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // ❗ Cascade değil

            // 📅 Appointment → Service
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict); // ❗ Cascade değil

            modelBuilder.Entity<UserSettings>()
             .HasOne(us => us.User)
             .WithOne(u => u.UserSettings)
             .HasForeignKey<UserSettings>(us => us.UserId);

            base.OnModelCreating(modelBuilder);
        }
        // Diğer tablolar da burada olacak:
        // public DbSet<Product> Products { get; set; }
        // public DbSet<Appointment> Appointments { get; set; }
    }
}
