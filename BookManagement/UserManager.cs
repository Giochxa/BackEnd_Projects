namespace BookManagement
{
    using BookManagement;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    public class UserManager
    {
        private List<User> users;
        private User currentUser;
        private const string FileName = "users.json";

        public UserManager()
        {
            users = LoadUsersFromFile();
        }

        // Register a new user
        public bool Register(string username, string password)
        {
            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Please choose a different one.");
                return false;
            }

            users.Add(new User(username, password));
            SaveUsersToFile();
            Console.WriteLine("Registration successful!");
            return true;
        }

        // User login
        public bool Login(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                currentUser = user;
                Console.WriteLine("Login successful!");
                return true;
            }

            Console.WriteLine("Invalid username or password.");
            return false;
        }

        // User logout
        public void Logout()
        {
            currentUser = null;
            Console.WriteLine("Logged out.");
        }

        // Check if a user is logged in
        public bool IsUserLoggedIn()
        {
            return currentUser != null;
        }

        // Save user data to a JSON file
        private void SaveUsersToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        // Load user data from a JSON file
        private List<User> LoadUsersFromFile()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    string json = File.ReadAllText(FileName);
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }

            return new List<User>();
        }
        public string GetCurrentUsername()
        {
            return currentUser?.Username;
        }

    }

}
