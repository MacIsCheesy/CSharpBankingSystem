using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class User {
    public string Username {get; set;}
    public string Password {get; set;}
    public double Balance {get; set;} = 0.0;
    public List<string> TransactionHistory { get; set; } = new List<string>();
}
class Program {

    static string GetDesktop(string filename) {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        return Path.Combine(desktopPath, filename);
    }
    static void CreateAccount() {
        Console.Write("Enter a username: ");
        string username = Console.ReadLine();
        Console.Write("Enter a password: ");
        string password = Console.ReadLine();

        User user = new User { Username = username, Password = password };
        string json = JsonSerializer.Serialize(user);
        File.WriteAllText(GetDesktop($"{username}.json"), json);
        Console.WriteLine("Account created successfully!");
    }
    static void Login() {

        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        DateTime now = DateTime.Now;


        if (!File.Exists(GetDesktop($"{username}.json"))) {
            Console.WriteLine("User not found. Please create an account first.");
            return;
        }
        Console.Write("Enter your password: ");
        string password = Console.ReadLine();

        string json = File.ReadAllText(GetDesktop($"{username}.json"));
        User user = JsonSerializer.Deserialize<User>(json);

        if (user.Password == password) {
            Console.WriteLine($"Welcome, {username}!");
            while (true) {
                Console.WriteLine("\n1. Check Balance\n2. Deposit\n3. Withdraw\n4. Transfer Funds\n5. View Transaction History\n6. Logout");
                Console.Write("Input: ");
                string choice = Console.ReadLine();

                switch (choice) {
                    case "1":
                        Console.WriteLine($"Your balance is: {user.Balance}");
                        break;
                    case "2":
                        Console.Write("Input amount to deposit: ");
                        double depositAmount = double.Parse(Console.ReadLine());
                        user.Balance += depositAmount;
                        user.TransactionHistory.Add($"\nOn {now.ToString("dd-MM-yyyy HH:mm")}\nDeposited {depositAmount}\n");
                        Console.WriteLine($"Successfully deposited {depositAmount} from account. New balance is {user.Balance}");
                        break;
                    case "3":
                        Console.Write("Input amount to withdraw: ");
                        double withdrawAmount = double.Parse(Console.ReadLine());
                        if (withdrawAmount > user.Balance) {
                            Console.WriteLine("Insufficient balance.");
                        }
                        else {
                            user.Balance -= withdrawAmount;
                            user.TransactionHistory.Add($"\nOn {now.ToString("dd-MM-yyyy HH:mm")}\nWithdrew {withdrawAmount}\n");
                            Console.WriteLine($"Successfully withdrew {withdrawAmount}. New balance is {user.Balance}");
                        }
                        break;
                    case "4":
                        TransferFunds(user);
                        break;
                    case "5":
                        ViewTransactionHistory(user);
                        break;
                    case "6":
                        json = JsonSerializer.Serialize(user);
                        File.WriteAllText(GetDesktop($"{username}.json"), json);
                        Console.WriteLine("Logged out successfully!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine("Incorrect password. Please try again.");
        }
    }
    static void TransferFunds(User sender) {

        Console.Write("Enter the recipient's username: ");
        string recipientUsername = Console.ReadLine();
        DateTime now = DateTime.Now;

        if (!File.Exists(GetDesktop($"{recipientUsername}.json"))) {
            Console.WriteLine("Recipient account not found.");
            return;
        }
        Console.Write("Enter the amount to transfer: ");
        double transferAmount = double.Parse(Console.ReadLine());
        
        if (transferAmount > sender.Balance) {
            Console.WriteLine("Insufficient balance.");
            return;
        }
        string json = File.ReadAllText(GetDesktop($"{recipientUsername}.json"));
        User recipient = JsonSerializer.Deserialize<User>(json);
        sender.Balance -= transferAmount;
        recipient.Balance += transferAmount;

        sender.TransactionHistory.Add($"\nOn {now.ToString("dd-MM-yyyy HH:mm")} Sent {transferAmount} to {recipientUsername}\n");
        recipient.TransactionHistory.Add($"\nOn {now.ToString("dd-MM-yyyy HH:mm")} Received {transferAmount} from {sender.Username}\n");
        File.WriteAllText(GetDesktop($"{sender.Username}.json"), JsonSerializer.Serialize(sender));
        File.WriteAllText(GetDesktop($"{recipientUsername}.json"), JsonSerializer.Serialize(recipient));
        Console.WriteLine($"Transferred {transferAmount} to {recipient.Username}. New balance is {sender.Balance}");
    }
    static void ViewTransactionHistory(User user) {
        Console.WriteLine("\nTransaction History:");
        foreach (var transaction in user.TransactionHistory) {
            Console.WriteLine(transaction);
        }
    }

    static void Main(string[] args) {
        while (true) {
            Console.WriteLine("Welcome, please follow the instructions below:\n1. Create Account\n2. Login\n3. Exit");
            Console.Write("Input: ");
            string choice = Console.ReadLine();
            switch (choice) {
                case "1":
                    CreateAccount();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    Console.WriteLine("Exiting the system. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}