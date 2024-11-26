using BookManagement;

namespace BookManagement
{
    class Program
    {
        static void Main()
        {
            var bookManager = new BookManager();
            var userManager = new UserManager();
            string input;

            do
            {
                Console.WriteLine("\n Book Management System");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Add a new book");
                Console.WriteLine("4. View all books");
                Console.WriteLine("5. Search books by title");
                Console.WriteLine("6. Rate a book");
                Console.WriteLine("7. Logout");
                Console.WriteLine("8. Exit");
                Console.Write("Choose an option (1-8): ");
                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        RegisterUser(userManager);
                        break;
                    case "2":
                        LoginUser(userManager);
                        break;
                    case "3":
                        AddNewBook(bookManager);
                        break;
                    case "4":
                        bookManager.ListAllBooks();
                        break;
                    case "5":
                        SearchBooks(bookManager);
                        break;
                    case "6":
                        RateBook(bookManager, userManager);
                        break;
                    case "7":
                        userManager.Logout();
                        break;
                    case "8":
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            } while (input != "8");
        }

        static void RegisterUser(UserManager userManager)
        {
            Console.Write("Enter a username: ");
            string username = Console.ReadLine();
            Console.Write("Enter a password: ");
            string password = Console.ReadLine();
            userManager.Register(username, password);
        }

        static void LoginUser(UserManager userManager)
        {
            Console.Write("Enter your username: ");
            string username = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            userManager.Login(username, password);
        }

        static void AddNewBook(BookManager bookManager)
        {
            // Enter and validate the title of the book
            Console.Write("Enter the title of the book: ");
            string title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Invalid title. The title cannot be empty.");
                AddNewBook(bookManager);
            }

            // Enter and validate the author of the book
            Console.Write("Enter the author of the book: ");
            string author = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("Invalid author. The author's name cannot be empty.");
                return;
            }

            // Enter and validate the issuance year of the book
            Console.Write("Enter the issuance year of the book: ");
            if (int.TryParse(Console.ReadLine(), out int issuanceYear))
            {
                // Validate that the year is reasonable
                int currentYear = DateTime.Now.Year;
                if (issuanceYear < 1450 || issuanceYear > currentYear)
                {
                    Console.WriteLine($"Invalid year. Please enter a year between 1450 and {currentYear}.");
                    return;
                }

                // Create a new Book object and pass it to AddBook
                var newBook = new Book(title, author, issuanceYear);
                bookManager.AddBook(newBook);
                Console.WriteLine("Book successfully added!");
            }
            else
            {
                Console.WriteLine("Invalid year, please enter a valid number.");
            }
        }



        static void SearchBooks(BookManager bookManager)
        {
            Console.Write("Enter the title to search: ");
            string title = Console.ReadLine();
            bookManager.SearchBooksByTitle(title);
        }

        static void RateBook(BookManager bookManager, UserManager userManager)
        {
            if (!userManager.IsUserLoggedIn())
            {
                Console.WriteLine("\nYou must be logged in to rate a book.✰★☆✮✭✬");
                return;
            }

            Console.Write("Enter the title of the book to rate: ");
            string title = Console.ReadLine();

            Console.Write("Enter a rating (1-5): ");
            if (int.TryParse(Console.ReadLine(), out int rating))
            {
                string username = userManager.GetCurrentUsername(); // Get the logged-in user's username
                bookManager.RateBook(title, username, rating); // Rating is saved to the JSON file
            }
            else
            {
                Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
            }
        }



    }

}