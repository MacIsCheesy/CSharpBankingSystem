using System;
using System.IO;
using System.Text.Json;

public class User {
    public string Username { get; set; }
    public string Password { get; set; }
    public double Balance { get; set; } = 0.0;
}
class Program {

    static string GetDesktopPath(string filename) {
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
        File.WriteAllText(GetDesktopPath($"{username}.json"), json);

        Console.WriteLine("Account created successfully!");
    }
    static void Login() {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();

        if (!File.Exists(GetDesktopPath($"{username}.json"))) {
            Console.WriteLine("User not found. Please create an account first.");
            return;
        }
        Console.Write("Enter your password: ");
        string password = Console.ReadLine();

        string json = File.ReadAllText(GetDesktopPath($"{username}.json"));
        User user = JsonSerializer.Deserialize<User>(json);

        if (user.Password == password) {
            Console.WriteLine($"Welcome, {username}!");
            while (true) {
                Console.WriteLine("\n1. Check Balance\n2. Deposit\n3. Withdraw\n4. Transfer funds\n5. Logout");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice) {
                    case "1":
                        Console.WriteLine($"Your balance is: {user.Balance}");
                        break;
                    case "2":
                        Console.Write("Enter the amount to deposit: ");
                        double depositAmount = double.Parse(Console.ReadLine());
                        user.Balance += depositAmount;
                        Console.WriteLine($"Successfully deposited {depositAmount}. New balance is {user.Balance}");
                        break;
                    case "3":
                        Console.Write("Enter the amount to withdraw: ");
                        double withdrawAmount = double.Parse(Console.ReadLine());
                        if (withdrawAmount > user.Balance) {
                            Console.WriteLine("Insufficient balance.");
                        }
                        else {
                            user.Balance -= withdrawAmount;
                            Console.WriteLine($"Successfully withdrew {withdrawAmount}. New balance is {user.Balance}");
                        }
                        break;
                    case "4":
                        TransferFunds(user);
                        break;
                    case "5":
                        json = JsonSerializer.Serialize(user);
                        File.WriteAllText(GetDesktopPath($"{username}.json"), json);
                        Console.WriteLine("Logged out successfully!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        else {
            Console.WriteLine("Incorrect password. Please try again.");
        }
    }
    static void TransferFunds(User sender) {
        Console.Write("Enter recipient's username: ");
        string recipientUsername = Console.ReadLine();

        if (!File.Exists(GetDesktopPath($"{recipientUsername}.json"))) {
            Console.Write("Recipient's name not found in our database.");
            return;
        }

        Console.Write("Enter the amount to transfer: ");
        double transferAmount = double.Parse(Console.ReadLine());

        if (transferAmount > sender.Balance) {
            Console.WriteLine("Insufficient balance.");
            return;
        }

        string json = File.ReadAllText(GetDesktopPath($"{recipientUsername}.json"));
        User recipient = JsonSerializer.Deserialize<User>(json);
        sender.Balance -= transferAmount;
        recipient.Balance += transferAmount;

        File.WriteAllText(GetDesktopPath($"{sender.Username}.json"), JsonSerializer.Serialize(sender));
        File.WriteAllText(GetDesktopPath($"{recipientUsername}.json"), JsonSerializer.Serialize(recipient));
        Console.WriteLine($"Successful transfer of {transferAmount} to {recipientUsername}. New balance is {sender.Balance}");
    }
    static void Main(string[] args) {
        while (true) {
            Console.WriteLine("Welcome, please follow the instructions on the screen.");
            Console.WriteLine("\n1. Create Account\n2. Login\n3. Exit");
            Console.Write("Enter your choice: ");
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
