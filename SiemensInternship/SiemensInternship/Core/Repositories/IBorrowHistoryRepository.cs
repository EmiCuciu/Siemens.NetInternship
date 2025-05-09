using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Repositories;

public interface IBorrowHistoryRepository
{
    Task AddAsync(BorrowHistory history);
    Task<List<BorrowHistory>> GetUserHistoryAsync(int userId);
    Task<List<Book>> GetFrequentlyBorrowedBooksAsync(int count);
    Task<List<BorrowHistory>> GetOverdueLoansAsync();
    Task<BorrowHistory?> GetActiveLoanAsync(int bookId, int userId);
}