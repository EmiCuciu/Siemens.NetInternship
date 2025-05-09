using LibraryManagement.Core.Models;

namespace LibraryManagement.Core.Services;

public interface IRecommendationService
{
    Task<List<Book>> GetRecommendations(int userId);
    Task<List<Book>> GetPopularBooks(int count);
}