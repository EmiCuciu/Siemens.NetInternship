using LibraryManagement.Core.Data;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Repositories;
using LibraryManagement.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
ConfigureServices(services);
var serviceProvider = services.BuildServiceProvider();

var bookService = serviceProvider.GetRequiredService<IBookService>();
var recommendationService = serviceProvider.GetRequiredService<IRecommendationService>();

while (true)
{
    Console.Clear();
    DisplayMenu();
    await ProcessChoice(await GetUserChoice(), bookService, recommendationService);
}

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<LibraryDbContext>(options =>
        options.UseSqlite("Data Source=Library.db"));

    services.AddScoped<IBookRepository, BookRepository>();
    services.AddScoped<IBorrowHistoryRepository, BorrowHistoryRepository>();
    services.AddScoped<IBookService, BookService>();
    services.AddScoped<IRecommendationService, RecommendationService>();
}

void DisplayMenu()
{
    Console.WriteLine("=== LIBRARY MANAGEMENT SYSTEM ===");
    Console.WriteLine("1. Add Book");
    Console.WriteLine("2. View All Books");
    Console.WriteLine("3. Search Books");
    Console.WriteLine("4. Borrow Book");
    Console.WriteLine("5. Return Book");
    Console.WriteLine("6. Update Book");
    Console.WriteLine("7. Delete Book");
    Console.WriteLine("8. Get Recommendations");
    Console.WriteLine("9. View Overdue Loans");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice (0-9): ");
}

async Task<int> GetUserChoice()
{
    while (true)
    {
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= 9)
            return choice;
        Console.Write("Invalid input. Please enter a number (0-9): ");
    }
}

async Task ProcessChoice(int choice, IBookService bookService, IRecommendationService recommendationService)
{
    try
    {
        switch (choice)
        {
            case 1: await AddBook(bookService); break;
            case 2: await ViewAllBooks(bookService); break;
            case 3: await SearchBooks(bookService); break;
            case 4: await BorrowBook(bookService); break;
            case 5: await ReturnBook(bookService); break;
            case 6: await UpdateBook(bookService); break;
            case 7: await DeleteBook(bookService); break;
            case 8: await ShowRecommendations(recommendationService); break;
            case 9: await ViewOverdueLoans(bookService); break;
            case 0: Environment.Exit(0); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

async Task AddBook(IBookService service)
{
    Console.WriteLine("\n=== ADD NEW BOOK ===");

    Console.Write("Title: ");
    var title = Console.ReadLine()!;

    Console.Write("Author: ");
    var author = Console.ReadLine()!;

    var book = new Book(title, author)
    {
        Quantity = 1,
        AvailableCopies = 1
    };

    Console.Write("Quantity (default 1): ");
    if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
        book.Quantity = quantity;

    Console.Write("Publication Year (optional): ");
    if (int.TryParse(Console.ReadLine(), out int year))
        book.PublicationYear = year;

    Console.Write("Category (optional): ");
    book.Category = Console.ReadLine();

    Console.Write("Description (optional): ");
    book.Description = Console.ReadLine();

    await service.AddAsync(book);
    Console.WriteLine("Book added successfully!");
}

async Task ViewAllBooks(IBookService service)
{
    Console.WriteLine("\n=== ALL BOOKS ===");
    var books = await service.GetAllAsync();

    foreach (var book in books)
    {
        Console.WriteLine($"ID: {book.Id}");
        Console.WriteLine($"Title: {book.Title}");
        Console.WriteLine($"Author: {book.Author}");
        Console.WriteLine($"Available: {book.AvailableCopies}/{book.Quantity}");
        Console.WriteLine($"Year: {book.PublicationYear}");
        Console.WriteLine($"Category: {book.Category}");
        Console.WriteLine($"Description: {book.Description}");
        Console.WriteLine("---------------------");
    }
}

async Task SearchBooks(IBookService service)
{
    Console.WriteLine("\n=== SEARCH BOOKS ===");
    Console.Write("Title (leave empty to skip): ");
    var title = Console.ReadLine();

    Console.Write("Author (leave empty to skip): ");
    var author = Console.ReadLine();

    Console.Write("Category (leave empty to skip): ");
    var category = Console.ReadLine();

    Console.Write("Publication Year (0 to skip): ");
    int.TryParse(Console.ReadLine(), out int year);


    var results = await service.SearchBooksAsync(
        string.IsNullOrWhiteSpace(title) ? null : title,
        string.IsNullOrWhiteSpace(author) ? null : author,
        string.IsNullOrWhiteSpace(category) ? null : category,
        year == 0 ? null : (int?)year);

    if (!results.Any())
    {
        Console.WriteLine("No books found matching your search.");
        return;
    }

    Console.WriteLine("\nSearch Results:");
    foreach (var book in results)
    {
        Console.WriteLine($"{book.Id}. {book.Title} by {book.Author}");
        Console.WriteLine($"    Category: {book.Category}, Year: {book.PublicationYear}");
        Console.WriteLine($"    Available: {book.AvailableCopies}/{book.Quantity}");
    }
}

async Task BorrowBook(IBookService service)
{
    Console.WriteLine("\n=== BORROW BOOK ===");
    await ViewAllBooks(service);

    Console.Write("\nEnter Book ID to borrow: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid Book ID");
        return;
    }

    Console.Write("Enter Your User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID");
        return;
    }

    await service.BorrowBook(bookId, userId);
    Console.WriteLine("Book borrowed successfully!");
}

async Task ReturnBook(IBookService service)
{
    Console.WriteLine("\n=== RETURN BOOK ===");
    await ViewAllBooks(service);

    Console.Write("\nEnter Book ID to return: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid Book ID");
        return;
    }

    Console.Write("Enter Your User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID");
        return;
    }

    await service.ReturnBook(bookId, userId);
    Console.WriteLine("Book returned successfully!");
}

async Task UpdateBook(IBookService service)
{
    Console.WriteLine("\n=== UPDATE BOOK ===");
    await ViewAllBooks(service);

    Console.Write("\nEnter Book ID to update: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid Book ID");
        return;
    }

    var book = await service.GetByIdAsync(bookId);
    if (book == null)
    {
        Console.WriteLine("Book not found");
        return;
    }

    Console.WriteLine("\nCurrent Book Details:");
    Console.WriteLine($"Title: {book.Title}");
    Console.WriteLine($"Author: {book.Author}");
    Console.WriteLine($"Quantity: {book.Quantity}");
    Console.WriteLine($"Publication Year: {book.PublicationYear}");
    Console.WriteLine($"Category: {book.Category}");
    Console.WriteLine($"Description: {book.Description}");

    Console.WriteLine("\nEnter new details (leave blank to keep current value):");

    Console.Write($"Title [{book.Title}]: ");
    var title = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(title))
        book.Title = title;

    Console.Write($"Author [{book.Author}]: ");
    var author = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(author))
        book.Author = author;

    Console.Write($"Quantity [{book.Quantity}]: ");
    var quantityStr = Console.ReadLine();
    if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
        book.Quantity = quantity;

    Console.Write($"Publication Year [{book.PublicationYear}]: ");
    var yearStr = Console.ReadLine();
    if (int.TryParse(yearStr, out int year))
        book.PublicationYear = year;

    Console.Write($"Category [{book.Category}]: ");
    var category = Console.ReadLine();
    if (category != null)
        book.Category = category;

    Console.Write($"Description [{book.Description}]: ");
    var description = Console.ReadLine();
    if (description != null)
        book.Description = description;

    await service.UpdateAsync(book);
    Console.WriteLine("Book updated successfully!");
}

async Task DeleteBook(IBookService service)
{
    Console.WriteLine("\n=== DELETE BOOK ===");
    await ViewAllBooks(service);

    Console.Write("\nEnter Book ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int bookId))
    {
        Console.WriteLine("Invalid Book ID");
        return;
    }

    await service.DeleteAsync(bookId);
    Console.WriteLine("Book deleted successfully!");
}

async Task ShowRecommendations(IRecommendationService service)
{
    Console.WriteLine("\n=== BOOK RECOMMENDATIONS ===");
    Console.Write("Enter Your User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID");
        return;
    }

    var recommendations = await service.GetRecommendations(userId);

    if (!recommendations.Any())
    {
        Console.WriteLine("No recommendations available. Try borrowing some books first!");
        return;
    }

    Console.WriteLine("\nWe recommend these books for you:");
    foreach (var book in recommendations)
    {
        Console.WriteLine($"- {book.Title} by {book.Author} ({book.Category})");
    }
}

async Task ViewOverdueLoans(IBookService service)
{
    Console.WriteLine("\n=== OVERDUE LOANS ===");
    var overdueLoans = await service.GetOverdueLoansAsync();

    if (!overdueLoans.Any())
    {
        Console.WriteLine("No overdue loans found.");
        return;
    }

    foreach (var loan in overdueLoans)
    {
        Console.WriteLine($"Book: {loan.Book?.Title}");
        Console.WriteLine($"Borrowed by User ID: {loan.UserId}");
        Console.WriteLine($"Borrow Date: {loan.BorrowDate:yyyy-MM-dd}");
        Console.WriteLine($"Days Overdue: {(DateTime.UtcNow - loan.BorrowDate).TotalDays:F0}");
        Console.WriteLine("---------------------");
    }
}