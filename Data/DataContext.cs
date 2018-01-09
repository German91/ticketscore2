using Microsoft.EntityFrameworkCore;
using Tickets.API.Models;

namespace Tickets.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(t => t.Tickets)
                .WithOne(u => u.Owner)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}