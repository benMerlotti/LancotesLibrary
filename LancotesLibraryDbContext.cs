using Microsoft.EntityFrameworkCore;
using LancotesLibrary.Models;

public class LancotesLibraryDbContext : DbContext
{

    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<Patron> Patrons { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Genre> Genres { get; set; }

    public LancotesLibraryDbContext(DbContextOptions<LancotesLibraryDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed Data for Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Fiction" },
            new Genre { Id = 2, Name = "Non-Fiction" },
            new Genre { Id = 3, Name = "Science Fiction" },
            new Genre { Id = 4, Name = "Biography" }
        );

        // Seed Data for Material Types
        modelBuilder.Entity<MaterialType>().HasData(
            new MaterialType { Id = 1, Name = "Book", CheckoutDays = 14 },
            new MaterialType { Id = 2, Name = "DVD", CheckoutDays = 7 },
            new MaterialType { Id = 3, Name = "Magazine", CheckoutDays = 3 }
        );

        // Seed Data for Materials
        modelBuilder.Entity<Material>().HasData(
            new Material { Id = 1, MaterialName = "The Great Gatsby", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null },
            new Material { Id = 2, MaterialName = "Inception", MaterialTypeId = 2, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 3, MaterialName = "National Geographic: August 2023", MaterialTypeId = 3, GenreId = 2, OutOfCirculationSince = new DateTime(2024, 1, 1) },
            new Material { Id = 4, MaterialName = "1984", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null },
            new Material { Id = 5, MaterialName = "The Matrix", MaterialTypeId = 2, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 6, MaterialName = "Forbes: January 2024", MaterialTypeId = 3, GenreId = 2, OutOfCirculationSince = null },
            new Material { Id = 7, MaterialName = "The Catcher in the Rye", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null },
            new Material { Id = 8, MaterialName = "Interstellar", MaterialTypeId = 2, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 9, MaterialName = "TIME: December 2023", MaterialTypeId = 3, GenreId = 2, OutOfCirculationSince = null },
            new Material { Id = 10, MaterialName = "The Hobbit", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = new DateTime(2023, 6, 15) }

        );

        // Seed Data for Patrons
        modelBuilder.Entity<Patron>().HasData(
            new Patron { Id = 1, FirstName = "Alice", LastName = "Johnson", Address = "123 Library Lane", Email = "alice.johnson@example.com", IsActive = true },
            new Patron { Id = 2, FirstName = "Bob", LastName = "Smith", Address = "456 Book Blvd", Email = "bob.smith@example.com", IsActive = false },
            new Patron { Id = 3, FirstName = "Carol", LastName = "Williams", Address = "789 Novel Ave", Email = "carol.williams@example.com", IsActive = true }
        );
    }
}