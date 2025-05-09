using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Services;

public interface IBookService
{
    Task<Book?> GetByIdAsync(int id);
    Task<List<Book>> GetAllAsync();
    Task<List<Book>> SearchBooksAsync(string? title, string? author, string? category, int? year);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task BorrowBook(int bookId, int userId);
    Task ReturnBook(int bookId, int userId);
    Task<List<BorrowHistory>> GetOverdueLoansAsync();
}