using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookManagement
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int IssuanceYear { get; set; }

        // Use a dictionary to store user ratings (key: username, value: rating)
        [JsonInclude]
        public Dictionary<string, int> Ratings { get; private set; } = new Dictionary<string, int>();

        public Book(string title, string author, int issuanceYear)
        {
            Title = title;
            Author = author;
            IssuanceYear = issuanceYear;
            Ratings = new Dictionary<string, int>();
        }

        // Add or update a rating
        public void AddOrUpdateRating(string username, int rating)
        {
            if (rating < 1 || rating > 5)
            {
                Console.WriteLine("Rating must be between 1 and 5.");
                return;
            }

            if (Ratings.ContainsKey(username))
            {
                Console.WriteLine($"Updating rating for {username}.");
                Ratings[username] = rating;
            }
            else
            {
                Console.WriteLine($"Adding new rating for {username}.");
                Ratings.Add(username, rating);
            }
        }

        // Calculate the average rating of the book
        public double GetAverageRating()
        {
            if (Ratings.Count == 0) return 0.0;
            return Ratings.Values.Average();
        }

        public override string ToString()
        {
            double averageRating = GetAverageRating();
            return $"Title: {Title}, Author: {Author}, Year: {IssuanceYear}, Average Rating: {averageRating:F1} ({Ratings.Count} ratings)";
        }
    }
}
