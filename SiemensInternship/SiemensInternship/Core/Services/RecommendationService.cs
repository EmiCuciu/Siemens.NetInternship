using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;

namespace LibraryManagement.Core.Services;

public class RecommendationService(
    IBookRepository bookRepository,
    IBorrowHistoryRepository borrowHistoryRepository) : IRecommendationService
{
    public async Task<List<Book>> GetRecommendations(int userId)
    {
        var userHistory = await borrowHistoryRepository.GetUserHistoryAsync(userId);
        var popularBooks = await GetPopularBooks(5);
        var allBooks = await bookRepository.GetAllAsync();

        return allBooks
            .Where(b => !userHistory.Any(uh => uh.BookId == b.Id))
            .OrderByDescending(b => GetRecommendationScore(b, userHistory, popularBooks))
            .Take(5)
            .ToList();
    }

    public async Task<List<Book>> GetPopularBooks(int count)
    {
        return await borrowHistoryRepository.GetFrequentlyBorrowedBooksAsync(count);
    }

    private static int GetRecommendationScore(Book book,
        List<BorrowHistory> userHistory,
        List<Book> popularBooks)
    {
        int score = 0;

        // Score based on user's preferred categories
        score += userHistory.Count(uh => uh.Book?.Category == book.Category) * 3;

        // Score based on user's preferred authors
        score += userHistory.Count(uh => uh.Book?.Author == book.Author) * 2;

        // Score if book is popular
        score += popularBooks.Any(pb => pb.Id == book.Id) ? 2 : 0;

        // Score if book is new
        score += (book.PublicationYear >= DateTime.Now.Year - 2) ? 1 : 0;

        return score;
    }
}