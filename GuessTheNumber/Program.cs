using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GuessTheNumber
{
    class Program
    {
        static void Main()
        {
            // File path for saving scores
            string filePath = "highscores.txt";

            bool playAgain;

            do
            {
                DisplayHighScores(filePath);

                // Define the range for the random number
                int minRange = 1;
                int maxRange = 100;

                // Initialize random number generator
                Random random = new Random();
                int numberToGuess = random.Next(minRange, maxRange + 1);
                int userGuess = 0;
                int numberOfTries = 0;

                // Ask the user for their name
                Console.Write("\nEnter your name (or leave blank for Anonymous): ");
                string userName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = "Anonymous";
                }

                Console.WriteLine($"Hello, {userName}! Guess the number between {minRange} and {maxRange}!");

                // Loop until the user guesses the correct number
                while (userGuess != numberToGuess)
                {
                    Console.Write("Enter your guess: ");
                    string userInput = Console.ReadLine();

                    // Validate if the input is a number
                    if (int.TryParse(userInput, out userGuess))
                    {
                        // Check if the guess is within the specified range
                        if (userGuess < minRange || userGuess > maxRange)
                        {
                            Console.WriteLine($"Your guess is out of range! Please enter a number between {minRange} and {maxRange}.");
                            continue; // Skip to the next iteration without counting the try
                        }

                        numberOfTries++; // Increment the count only if the guess is valid

                        // Provide feedback to the user
                        if (userGuess < numberToGuess)
                        {
                            Console.WriteLine("Too low! Try a higher number.");
                        }
                        else if (userGuess > numberToGuess)
                        {
                            Console.WriteLine("Too high! Try a lower number.");
                        }
                        else
                        {
                            Console.WriteLine($"Congratulations, {userName}! You've guessed the number in {numberOfTries} tries.");

                            // Save the user's score
                            SaveScore(userName, numberOfTries, filePath);

                            // Display high scores
                            DisplayHighScores(filePath);

                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                    }
                }

                // Ask if the user wants to play again
                Console.Write("Do you want to play again? (Y/N): ");
                string playAgainInput = Console.ReadLine();
                playAgain = playAgainInput.Trim().ToUpper() == "Y";
                if (playAgain)
                {
                    Console.Clear();
                }
            } while (playAgain);

            Console.WriteLine("Thank you for playing! Goodbye!");
        }

        // Method to save the score to a file
        static void SaveScore(string name, int tries, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{name},{tries}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving score: {ex.Message}");
            }
        }

        // Method to display high scores from the file
        static void DisplayHighScores(string filePath)
        {
            Console.WriteLine("\nTop 10 High Scores:");

            try
            {
                if (File.Exists(filePath))
                {
                    List<(string Name, int Tries)> scores = new List<(string, int)>();

                    // Read scores from file
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');

                            if (parts.Length == 2 && int.TryParse(parts[1], out int tries))
                            {
                                scores.Add((parts[0], tries));
                            }
                        }
                    }

                    // Sort scores by the number of tries (higher first)
                    var sortedScores = scores.OrderBy(s => s.Tries).ToList();

                    // Display only the top 10 scores
                    int position = 1;
                    foreach (var score in sortedScores.Take(10))
                    {
                        Console.WriteLine($"{position}. {score.Name}: {score.Tries} tries");
                        position++;
                    }

                }
                else
                {
                    Console.WriteLine("No high scores available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading high scores: {ex.Message}");
            }
        }
    }
}