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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Garage) // Указывает, что каждая машина имеет один гараж
                .WithMany(g => g.Cars) // Указывает, что один гараж может содержать много машин
                .HasForeignKey(c => c.GarageId); // Указывает на внешний ключ в таблице Car
        }
    }
}
