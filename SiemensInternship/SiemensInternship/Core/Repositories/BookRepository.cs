using LibraryManagement.Core.Data;
using LibraryManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Core.Repositories;

public class BookRepository(LibraryDbContext context) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(int id)
    {
        return await context.Books
            .Include(b => b.BorrowHistories)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await context.Books
            .AsNoTracking()
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<List<Book>> GetPopularBooksAsync(int count)
    {
        return await context.Books
            .OrderByDescending(b => b.BorrowHistories.Count)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Book>> SearchAsync(string? title, string? author, string? category, int? year)
    {
        var query = context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => EF.Functions.Like(b.Title, $"%{title}%"));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => EF.Functions.Like(b.Author, $"%{author}%"));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(b => EF.Functions.Like(b.Category!, $"%{category}%"));

        if (year.HasValue)
            query = query.Where(b => b.PublicationYear == year);

        return await query.ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        await context.Books.AddAsync(book);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        context.Books.Update(book);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await context.Books
            .Where(b => b.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<int> CountAsync()
    {
        return await context.Books.CountAsync();
    }
}