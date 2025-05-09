using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<List<Book>> GetAllAsync();
    Task<List<Book>> GetPopularBooksAsync(int count);
    Task<List<Book>> SearchAsync(string? title, string? author, string? category, int? year);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task<int> CountAsync();
}