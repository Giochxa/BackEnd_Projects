using BookManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class BookManager
{
    private List<Book> books;
    private const string FileName = "books.json";

    public BookManager()
    {
        books = LoadBooksFromFile();
    }

    // Add a new book
    public void AddBook(Book book)
    {
        books.Add(book);
        SaveBooksToFile();
    }

    // List all books
    public void ListAllBooks()
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books available.");
        }
        else
        {
            foreach (var book in books)
            {
                Console.WriteLine(book.ToString());
            }
        }
    }

    // Search books by title (containing string) and display results in console
    public void SearchBooksByTitle(string title)
    {
        var matchingBooks = books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matchingBooks.Any())
        {
            Console.WriteLine("\nBooks matching the search:\n");
            foreach (var book in matchingBooks)
            {
                Console.WriteLine(book.Title);
            }
        }
        else
        {
            Console.WriteLine("\nNo books found matching the search.");
        }
    }



    // Method to add or update a rating
    public void RateBook(string title, string username, int rating)
    {
        var book = books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (book != null)
        {
            // Check and update or add rating
            book.AddOrUpdateRating(username, rating);
            SaveBooksToFile();  // Save updated book data to JSON file
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
    }



    // Save books (including ratings) to a JSON file
    private void SaveBooksToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving books: {ex.Message}");
        }
    }

    // Load books (including ratings) from a JSON file
    private List<Book> LoadBooksFromFile()
    {
        try
        {
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading books: {ex.Message}");
        }

        return new List<Book>();
    }
}
