using LibraryManagement.Core.Data;
using LibraryManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Core.Repositories;

public class BorrowHistoryRepository(LibraryDbContext context) : IBorrowHistoryRepository
{
    public async Task AddAsync(BorrowHistory history)
    {
        await context.BorrowHistories.AddAsync(history);
        await context.SaveChangesAsync();
    }

    public async Task<List<BorrowHistory>> GetUserHistoryAsync(int userId)
    {
        return await context.BorrowHistories
            .Include(bh => bh.Book)
            .Where(bh => bh.UserId == userId)
            .OrderByDescending(bh => bh.BorrowDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Book>> GetFrequentlyBorrowedBooksAsync(int count)
    {
        return await context.Books
            .OrderByDescending(b => b.BorrowHistories.Count)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<BorrowHistory>> GetOverdueLoansAsync()
    {
        var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);
        return await context.BorrowHistories
            .Include(bh => bh.Book)
            .Where(bh => bh.ReturnDate == null && bh.BorrowDate < twoWeeksAgo)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<BorrowHistory?> GetActiveLoanAsync(int bookId, int userId)
    {
        return await context.BorrowHistories
            .Include(bh => bh.Book)
            .FirstOrDefaultAsync(bh =>
                bh.BookId == bookId &&
                bh.UserId == userId &&
                bh.ReturnDate == null);
    }
}