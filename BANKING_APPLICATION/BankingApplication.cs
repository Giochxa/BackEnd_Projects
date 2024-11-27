using NLog;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Schema;
using System.Transactions;
using System.Data.Common;
using System.Threading.Tasks;

namespace BANKING_APPLICATION
{
    public class BankingApplication
    {
        public static CardholderData Validation()
        {
            string jsonFilePath = "C:\\Users\\george.chkhaidze\\source\\repos\\BackEnd Projects\\BANKING_APPLICATION\\cardInfo.json";
            try
            {
                CardholderData[] userData = LoadUserData(jsonFilePath);

                if (userData != null)
                {
                    CardholderData validatedUser = ValidateCardInformation(userData);

                    if (validatedUser != null)
                    {
                        Console.WriteLine($"\nHello {validatedUser.firstName} {validatedUser.lastName}: \n");
                        return validatedUser; // Exit the program after successful validation
                    }

                    Console.WriteLine("Too many unsuccessful attempts! Wait for 30 seconds and try again.");
                    Thread.Sleep(30000);
                    Validation();
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }

            return null;
        }
        static CardholderData[] LoadUserData(string jsonFilePath)
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<CardholderData[]>(jsonContent);
                }

                Console.WriteLine("File not found: " + jsonFilePath);
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return null;
        }
        static CardholderData ValidateCardInformation(CardholderData[] userData)
        {
            const int maxAttempts = 3;
            try
            {
                for (int attempts = 0; attempts < maxAttempts; attempts++)
                {
                    Console.Write("Enter your card number: ");
                    string cardNumber = Console.ReadLine();

                    Console.Write("Enter your CVC: ");
                    string cvc = Console.ReadLine();

                    Console.Write("Enter your expiration date (MM/YY): ");
                    string expirationDate = Console.ReadLine();

                    foreach (var user in userData)
                    {
                        if (IsValidCardInformation(user, cardNumber, cvc, expirationDate) && ValidatePIN(user))
                        {
                            return user;
                        }
                    }

                    Console.WriteLine("Invalid card information. Please try again.\n");
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return null;
        }
        static bool IsValidCardInformation(CardholderData data, string cardNumber, string cvc, string expirationDate)
        {
            return cardNumber == data.cardDetails.cardNumber &&
                   cvc == data.cardDetails.CVC &&
                   expirationDate == data.cardDetails.expirationDate;
        }
        static bool ValidatePIN(CardholderData data)
        {
            const int maxPinAttempts = 3;
            try
            {
                for (int pinAttempts = 0; pinAttempts < maxPinAttempts; pinAttempts++)
                {
                    Console.Write("Enter your PIN: ");
                    string enteredPin = Console.ReadLine();

                    if (IsValidPIN(data, enteredPin))
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect PIN. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }

            return false;
        }
        static bool IsValidPIN(CardholderData data, string enteredPin)
        {
            return enteredPin == data.pinCode;
        }
        public static void Menu(CardholderData data)
        {
            Console.WriteLine("1. Check Deposit");
            Console.WriteLine("2. Get Amount");
            Console.WriteLine("3. Get Last 5 Transactions");
            Console.WriteLine("4. Add Amount");
            Console.WriteLine("5. Change Pin");
            Console.WriteLine("6. Change Amount");
            Console.WriteLine("7. Log Out\n");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CheckDeposit(data);
                    Menu(data);
                    break;
                case "2":
                    GetAmount(data);
                    var newUser = Validation();
                    Menu(newUser);
                    break;
                case "3":
                    GetTransactionHistory(data);
                    Menu(data);
                    break;
                case "4":
                    AddAmount(data);
                    var newUser1 = Validation();
                    Menu(newUser1);
                    break;
                case "5":
                    ChangePin(data);
                    Menu(data);
                    break;
                case "6":
                    ChangeAmount(data).Wait();
                    Menu(data);
                    break;
                case "7":
                    var newUser2 = Validation();
                    Menu(newUser2);
                    break;
                default:
                    Console.WriteLine("Invalid input \n");
                    Menu(data);
                    break;
            }
        }
        static void Writejsontransaction(CardholderData data, Transaction transaction, string type)
        {
            try
            {
                DateTime currentUtcTime = DateTime.UtcNow;
                string formattedTime = currentUtcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Assuming you have a CardholderData object with new transaction data
                CardholderData newData = new CardholderData
                {
                    firstName = data.firstName,
                    lastName = data.lastName,
                    cardDetails = new CardDetails
                    {
                        cardNumber = data.cardDetails.cardNumber,
                        expirationDate = data.cardDetails.expirationDate,
                        CVC = data.cardDetails.CVC
                    },
                    pinCode = data.pinCode,
                    transactionHistory = new Transaction[]
                    {
                    new Transaction
                    {
                    transactionDate = DateTime.UtcNow,
                    transactionType = type,
                    amount = transaction.amount,
                    amountGEL = transaction.amountGEL,
                    amountUSD = transaction.amountUSD,
                    amountEUR = transaction.amountEUR
                    }
                    }
                };

                // Specify the file path
                var filePath = "C:\\Users\\george.chkhaidze\\source\\repos\\BackEnd Projects\\BANKING_APPLICATION\\cardInfo.json";

                // Read existing data from the file
                string jsonContent = File.ReadAllText(filePath);
                CardholderData[] existingData = JsonConvert.DeserializeObject<CardholderData[]>(jsonContent);

                // Find the cardholder data to update
                CardholderData existingCardholder = Array.Find(existingData, card => card.cardDetails.cardNumber == newData.cardDetails.cardNumber);

                if (existingCardholder != null)
                {
                    existingCardholder.pinCode = newData.pinCode;
                    // Convert array to a list, add the new transaction, and convert back to an array
                    List<Transaction> updatedTransactions = new List<Transaction>(existingCardholder.transactionHistory);
                    updatedTransactions.InsertRange(0, newData.transactionHistory);
                    existingCardholder.transactionHistory = updatedTransactions.ToArray();
                }

                // Write the updated data back to the file
                string updatedJsonContent = JsonConvert.SerializeObject(existingData, Formatting.Indented);
                File.WriteAllText(filePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static Transaction CheckDeposit(CardholderData data)
        {
            Console.WriteLine("\nAmount is");
            try
            {
                foreach (var transaction in data.transactionHistory)
                {

                    Console.WriteLine($"Amount (GEL): {transaction.amountGEL}");
                    Console.WriteLine($"Amount (USD): {transaction.amountUSD}");
                    Console.WriteLine($"Amount (EUR): {transaction.amountEUR}");
                    Console.WriteLine();

                    Writejsontransaction(data, transaction, "Deposit Check");
                    return transaction; // Exit the loop after displaying Balance

                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return null;
        }
        static void GetAmount(CardholderData data)
        {
            try
            {
                Console.WriteLine("Please enter the amount you want to get or enter 'Exit' to exit:");
                var x = Console.ReadLine();
                if (x =="Exit" || x == "exit" || x == "EXIT")
                {
                    return;
                }
                else
                {
                    decimal.TryParse(x, out decimal amount);
                    var transaction1 = data.transactionHistory.FirstOrDefault();
                    if (amount > 0 && amount <= transaction1.amountGEL)
                    {
                            foreach (var transaction in data.transactionHistory)
                        {
                            
                            {
                                transaction.amount = amount;
                                transaction.amountGEL = transaction.amountGEL - amount;
                                Writejsontransaction(data, transaction, "Get Amount");
                                Console.WriteLine("Do you want a receipt? Y/N");
                                var receipt = Console.ReadLine();
                                if (receipt == "Y" || receipt == "y")
                                {
                                    CheckDeposit(data);
                                    return;
                                }
                                else if (receipt == "N" || receipt == "n")
                                {
                                    Console.WriteLine("Thanks for your effort in saving the environment");
                                    return;
                                }
                                else
                                    return;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Your balance: {transaction1.amountGEL} does not allow to withdrow amount of {amount}, try another amount!");
                        GetAmount(data);
                    }
                }          
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static void GetTransactionHistory(CardholderData user)
        {
            Console.WriteLine("\nTransaction History:\n");

            var counter = 0;
            try
            {
                foreach (var transaction in user.transactionHistory)
                {
                    Console.WriteLine($"Transaction Date: {transaction.transactionDate}");
                    Console.WriteLine($"Transaction Type: {transaction.transactionType}");
                    Console.WriteLine($"Amount : {transaction.amount}");
                    Console.WriteLine($"Amount (GEL): {transaction.amountGEL}");
                    Console.WriteLine($"Amount (USD): {transaction.amountUSD}");
                    Console.WriteLine($"Amount (EUR): {transaction.amountEUR}");
                    Console.WriteLine();
                    counter++;

                    if (counter == 5 || counter == user.transactionHistory.Length)
                    {
                        foreach (var trans in user.transactionHistory)
                        {
                            Writejsontransaction(user, transaction, "Get Transactions History");
                            return; // Exit the loop after displaying 5 transactions
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static void AddAmount(CardholderData data)
        {
            Console.WriteLine("Please enter the amount you want to add or enter 'Exit' to exit:\n");
            try
            {
                var x = Console.ReadLine();
                if (x == "Exit" || x == "exit" || x == "EXIT")
                {
                    return;
                }
                else
                {
                    decimal.TryParse(x, out decimal amount);
                    amount = Math.Round(amount, 2);
                    if (amount > 0)
                    {
                        foreach (var transaction in data.transactionHistory)
                        {
                            transaction.amount = amount;
                            transaction.amountGEL = transaction.amountGEL + amount;
                            Writejsontransaction(data, transaction, "Fill Amount");
                            Console.WriteLine($"The amount of {amount} has succesfuli addit to your GEL account\n");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Amunt has been entered!");
                        AddAmount(data);
                    }
                    return;
                }               
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static void ChangePin(CardholderData data)
        {
            Console.WriteLine("Please enter 4 digit new Pin Code or enter 'Exit' to exit: \n");
            try
            {
                var x = Console.ReadLine();
                if (x == "Exit" || x == "exit" || x == "EXIT")
                {
                    return;
                }
                else
                {
                    int.TryParse(x, out int pinCode);
                    if (pinCode.ToString().Length != 4)
                    {
                        ChangePin(data);
                    }
                    Console.WriteLine("Please Reenter new Pin Code\n");
                    int.TryParse(Console.ReadLine(), out int pinCode1);
                    if (pinCode == pinCode1)
                    {
                        data.pinCode = pinCode.ToString();
                        foreach (var transaction in data.transactionHistory)
                        {
                            Writejsontransaction(data, transaction, "Change Pin");
                            Console.WriteLine("\nPIN Code changed successfully!\n");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Pin Code has been entered!");
                        ChangePin(data);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return;
        }

        private static readonly Logger Logger = LogManager.GetLogger("fileLogger");

        // Method to handle currency conversion
        public static async Task ChangeAmount(CardholderData data)
        {
            // Initialize the CurrencyService to get live rates
            CurrencyService currencyService = new CurrencyService();
            List<CurrencyData> currencyData = await currencyService.FetchCurrencyDataAsync();

            // Check if the exchange rates were successfully retrieved
            if (currencyData == null)
            {
                Console.WriteLine("\nFailed to retrieve exchange rates. Please try again later.\n");
                return;
            }

            // Get the required exchange rates
            double? exchangeRateUSD = currencyService.GetExchangeRateByCode(currencyData, "USD");
            double? exchangeRateEUR = currencyService.GetExchangeRateByCode(currencyData, "EUR");

            // Check if exchange rates are available
            if (exchangeRateUSD == null || exchangeRateEUR == null)
            {
                Console.WriteLine("\nCould not retrieve necessary exchange rates. Please try again later.\n");
                return;
            }

            // Other conversion rates based on USD and EUR rates
            decimal exchangeRateUSDtoEUR = Math.Round((decimal)(exchangeRateEUR / exchangeRateUSD), 2);
            decimal exchangeRateEURtoUSD = Math.Round(1 / exchangeRateUSDtoEUR, 2);

            Console.WriteLine("Please enter the number of Currency you want to convert:\n");
            Console.WriteLine("1. GEL to USD");
            Console.WriteLine("2. GEL to EUR");
            Console.WriteLine("3. USD to GEL");
            Console.WriteLine("4. EUR to GEL");
            Console.WriteLine("5. USD to EUR");
            Console.WriteLine("6. EUR to USD");
            Console.WriteLine("7. Back to Menu");
            var num = Console.ReadLine();

            try
            {
                switch (num)
                {
                    case "1":
                        Console.WriteLine($"The convertion rate is: {exchangeRateUSD}\n");
                        ConvertCurrency(data, "GEL", "USD", (decimal)exchangeRateUSD);
                        break;
                    case "2":
                        Console.WriteLine($"The convertion rate is: {exchangeRateEUR}\n");
                        ConvertCurrency(data, "GEL", "EUR", (decimal)exchangeRateEUR);
                        break;
                    case "3":
                        Console.WriteLine($"The convertion rate is: {Math.Round((decimal)(1 / exchangeRateUSD), 2)}\n");
                        ConvertCurrency(data, "USD", "GEL", (decimal)(1 / exchangeRateUSD));
                        break;
                    case "4":
                        Console.WriteLine($"The convertion rate is: {Math.Round((decimal)(1 / exchangeRateEUR), 2)}\n");
                        ConvertCurrency(data, "EUR", "GEL", (decimal)(1 / exchangeRateEUR));
                        break;
                    case "5":
                        Console.WriteLine($"The convertion rate is: {exchangeRateUSDtoEUR}\n");
                        ConvertCurrency(data, "USD", "EUR", exchangeRateUSDtoEUR);
                        break;
                    case "6":
                        Console.WriteLine($"The convertion rate is: {exchangeRateEURtoUSD}\n");
                        ConvertCurrency(data, "EUR", "USD", exchangeRateEURtoUSD);
                        break;
                    case "7":
                        Menu(data);
                        return;
                    default:
                        Console.WriteLine("\nWrong input\n");
                        await ChangeAmount(data); // Use 'await' properly since it's now async
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error: {ex.Message}");
            }
        }

        // Helper method to perform the currency conversion
        private static void ConvertCurrency(CardholderData data, string fromCurrency, string toCurrency, decimal rate)
        {
            Console.WriteLine($"Please enter the amount of money to convert {fromCurrency} into {toCurrency}\n");
            decimal.TryParse(Console.ReadLine(), out decimal exchange);
            exchange = Math.Round(exchange, 2);

            if (exchange > 0)
            {
                foreach (var transaction in data.transactionHistory)
                {
                    if (IsSufficientFunds(transaction, fromCurrency, exchange))
                    {
                        PerformConversion(transaction, fromCurrency, toCurrency, exchange, rate);
                        Writejsontransaction(data, transaction, "Change Amount");
                        Console.WriteLine("\nThe amount has successfully converted\n");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                        ChangeAmount(data).Wait();
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("\nInvalid input\n");
                ChangeAmount(data).Wait();
                return;
            }
        }

        // Helper method to check if there are sufficient funds
        private static bool IsSufficientFunds(Transaction transaction, string currency, decimal amount)
        {
            return currency switch
            {
                "GEL" => amount < transaction.amountGEL,
                "USD" => amount < transaction.amountUSD,
                "EUR" => amount < transaction.amountEUR,
                _ => false,
            };
        }

        // Helper method to perform the actual conversion
        private static void PerformConversion(Transaction transaction, string fromCurrency, string toCurrency, decimal amount, decimal rate)
        {
            transaction.amount = amount;

            switch (fromCurrency)
            {
                case "GEL":
                    transaction.amountGEL -= amount;
                    break;
                case "USD":
                    transaction.amountUSD -= amount;
                    break;
                case "EUR":
                    transaction.amountEUR -= amount;
                    break;
            }

            decimal convertedAmount = Math.Round(amount / rate, 2);

            switch (toCurrency)
            {
                case "GEL":
                    transaction.amountGEL += convertedAmount;
                    break;
                case "USD":
                    transaction.amountUSD += convertedAmount;
                    break;
                case "EUR":
                    transaction.amountEUR += convertedAmount;
                    break;
            }
        }
    }

}