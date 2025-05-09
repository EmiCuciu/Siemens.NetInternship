namespace LibraryManagement.Core.Models;

public class Book : Entity<int>
{
    public Book(string title, string author)
    {
        Title = title;
        Author = author;
    }

    public string Title { get; set; }
    public string Author { get; set; }
    public int Quantity { get; set; } = 1;
    public int AvailableCopies { get; set; } = 1;
    public int? PublicationYear { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public ICollection<BorrowHistory> BorrowHistories { get; set; } = new List<BorrowHistory>();
}