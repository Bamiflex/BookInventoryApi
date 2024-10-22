
using Microsoft.EntityFrameworkCore;
public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();
 


modelBuilder.Entity<Book>()
            .Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Book>()
            .Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Book>()
            .Property(b => b.Genre)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(b => b.PublicationYear)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(13);

        modelBuilder.Entity<Book>()
            .Property(b => b.Description)
            .IsRequired(); 
    }
}
