using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Core.Services;

public class BookService(
    IBookRepository bookRepository,
    IBorrowHistoryRepository borrowHistoryRepository) : IBookService
{
    public async Task<Book?> GetByIdAsync(int id)
    {
        return await bookRepository.GetByIdAsync(id);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await bookRepository.GetAllAsync();
    }

    public async Task<List<Book>> SearchBooksAsync(string? title, string? author, string? category, int? year)
    {
        return await bookRepository.SearchAsync(title, author, category, year);
    }

    public async Task AddAsync(Book book)
    {
        await ValidateBook(book);
        await bookRepository.AddAsync(book);
    }

    public async Task UpdateAsync(Book book)
    {
        await ValidateBook(book);
        await bookRepository.UpdateAsync(book);
    }

    public async Task DeleteAsync(int id)
    {
        var book = await bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            throw new KeyNotFoundException("Book not found");
        }

        await bookRepository.DeleteAsync(id);
    }

    public async Task BorrowBook(int bookId, int userId)
    {
        var book = await bookRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException("Book not found");

        if (book.AvailableCopies <= 0)
        {
            throw new InvalidOperationException("No available copies to borrow");
        }

        var activeLoan = await borrowHistoryRepository.GetActiveLoanAsync(bookId, userId);
        if (activeLoan != null)
        {
            throw new InvalidOperationException("You already have this book on loan");
        }

        var history = new BorrowHistory(bookId, userId);
        history.SetBook(book);

        book.AvailableCopies--;
        await bookRepository.UpdateAsync(book);
        await borrowHistoryRepository.AddAsync(history);
    }

    public async Task ReturnBook(int bookId, int userId)
    {
        var book = await bookRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException("Book not found");

        var activeLoan = await borrowHistoryRepository.GetActiveLoanAsync(bookId, userId)
            ?? throw new InvalidOperationException("No active loan found for this book");

        if (book.AvailableCopies >= book.Quantity)
        {
            throw new InvalidOperationException("All copies are already available");
        }

        activeLoan.MarkReturned();
        book.AvailableCopies++;

        await bookRepository.UpdateAsync(book);
        await borrowHistoryRepository.AddAsync(activeLoan);
    }

    public async Task<List<BorrowHistory>> GetOverdueLoansAsync()
    {
        return await borrowHistoryRepository.GetOverdueLoansAsync();
    }

    private async Task ValidateBook(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
        {
            throw new ArgumentException("Title is required");
        }

        if (string.IsNullOrWhiteSpace(book.Author))
        {
            throw new ArgumentException("Author is required");
        }

        if (book.Quantity < 1)
        {
            throw new ArgumentException("Quantity must be at least 1");
        }

        if (book.PublicationYear.HasValue &&
            (book.PublicationYear < 1000 || book.PublicationYear > DateTime.Now.Year + 1))
        {
            throw new ArgumentException("Invalid publication year");
        }
    }
}