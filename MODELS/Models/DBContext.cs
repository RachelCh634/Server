using MODELS.Models;
using Microsoft.EntityFrameworkCore;
using Project;

namespace MODELS.Models
{
    public class DBContext : DbContext
    {
        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationsReceived> DonationsReceiveds { get; set; }
        public DbSet<UserDonationLike> UserDonationLikes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=RACHEL\\SQLEXPRESS;Database=SecondDB;Integrated Security=True;",
                    b => b.MigrationsAssembly("MODELS"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<User>()
           .Property(u => u.IdentityId)
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<Donation>()
           .Property(u => u.Id)
           .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }
    }
}