namespace LibraryManagement.Core.Models;

public class BorrowHistory : Entity<int>
{
    // Parameterless constructor for EF Core
    private BorrowHistory() { }

    public BorrowHistory(int bookId, int userId)
    {
        BookId = bookId;
        UserId = userId;
        BorrowDate = DateTime.UtcNow;
    }

    public int BookId { get; private set; }
    public Book Book { get; private set; } = null!;
    public int UserId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public void SetBook(Book book)
    {
        Book = book;
    }

    public void MarkReturned()
    {
        ReturnDate = DateTime.UtcNow;
    }
}