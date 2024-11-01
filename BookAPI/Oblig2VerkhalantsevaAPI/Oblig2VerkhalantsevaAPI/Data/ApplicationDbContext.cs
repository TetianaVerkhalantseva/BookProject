using Microsoft.EntityFrameworkCore;
using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Models.Entities;

namespace Oblig2VerkhalantsevaAPI.Data;

// dotnet ef migrations add <name of migration>
// dotnet ef database update

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    public DbSet<Book> Book { get; set; } = null!;
    public DbSet<Author> Author { get; set; } = null!;
    public DbSet<Category> Category { get; set; } = null!;
    public DbSet<Language> Language { get; set; } = null!;
    public DbSet<Publisher> Publisher { get; set; } = null!;


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tables
        modelBuilder.Entity<Book>().ToTable("Book");
        modelBuilder.Entity<Author>().ToTable("Author");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Language>().ToTable("Language");
        modelBuilder.Entity<Publisher>().ToTable("Publisher");

        // SEEDING

        // Authors
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "F. Scott", LastName = "Fitzgerald" },
            new Author { Id = 2, FirstName = "Harper", LastName = "Lee" },
            new Author { Id = 3, FirstName = "George", LastName = "Orwell" },
            new Author { Id = 4, FirstName = "Knut", LastName = "Hamsun" }
        );

        // Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Classic Literature" },
            new Category { Id = 2, Name = "Historical Fiction" },
            new Category { Id = 3, Name = "Dystopian" },
            new Category { Id = 4, Name = "Psychological Fiction" }
        );

        // Languages
        modelBuilder.Entity<Language>().HasData(
            new Language { Id = 1, Name = "English" },
            new Language { Id = 2, Name = "Norwegian" }
        );

        // Publishers
        modelBuilder.Entity<Publisher>().HasData(
            new Publisher { Id = 1, Name = "Scribner" },
            new Publisher { Id = 2, Name = "J.B. Lippincott & Co." },
            new Publisher { Id = 3, Name = "Secker & Warburg" },
            new Publisher { Id = 4, Name = "Gyldendal Norsk Forlag" }
        );

        // Books
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "The Great Gatsby", Description = "A classic novel by F. Scott Fitzgerald", Year = 1925, AuthorId = 1, CategoryId = 1, PublisherId = 1, LanguageId = 1 },
            new Book { Id = 2, Title = "To Kill a Mockingbird", Description = "A novel by Harper Lee about racial injustice", Year = 1960, AuthorId = 2, CategoryId = 2, PublisherId = 2, LanguageId = 1 },
            new Book { Id = 3, Title = "1984", Description = "A dystopian novel by George Orwell", Year = 1949, AuthorId = 3, CategoryId = 3, PublisherId = 3, LanguageId = 1 },
            new Book { Id = 4, Title = "Hunger", Description = "A psychological novel by Knut Hamsun", Year = 1890, AuthorId = 4, CategoryId = 4, PublisherId = 4, LanguageId = 2 }
        );
    }
}