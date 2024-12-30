using System;
using Microsoft.EntityFrameworkCore;
using OpenAPI.Models;

namespace OpenAPI.Context
{
    public class CarCatalogDbContext : DbContext
    {
        public CarCatalogDbContext(DbContextOptions<CarCatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Garage> Garages { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Garage) 
                .WithMany(g => g.Cars) 
                .HasForeignKey(c => c.GarageId); 
        }
    }
}
