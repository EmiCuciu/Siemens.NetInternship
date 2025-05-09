using LibraryManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Core.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BorrowHistory> BorrowHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BorrowHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BorrowDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(bh => bh.Book)
                .WithMany(b => b.BorrowHistories)
                .HasForeignKey(bh => bh.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(200);
        });
    }
}