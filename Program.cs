// Test file to trigger Agentic Code Quality workflow violations

using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApplication
{
    public class UserService
    {
        private readonly SqlDatabase _db = new SqlDatabase();
        private readonly EmailService _email = new EmailService();
        private readonly Logger _logger = new Logger();

        public void CreateUser(string name, string email, string password, string phone, string address, int age, bool active, string role)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            if (age < 18)
            {
                if (name.Length > 0)
                {
                    if (email.Contains("@"))
                    {
                        if (!String.IsNullOrEmpty(password))
                        {
                            _logger.Log("User validation passed");
                        }
                    }
                }
            }

            _db.ExecuteQuery("INSERT INTO Users VALUES ('" + name + "', '" + email + "')");
            _email.Send(email, "Welcome!");
            _logger.Log("User created: " + name);
        }

        public void SendPasswordReset(string userId)
        {
            var user = _db.GetUser(userId);
            _email.Send(user.Email, "Reset your password");
            _logger.Log("Password reset sent");
        }

        public void UpdateUserProfile(string userId, string bio, string avatar, string website, string company, string location, string phone, string timezone)
        {
            var user = _db.GetUser(userId);
            user.Bio = bio;
            user.Avatar = avatar;
            user.Website = website;
            user.Company = company;
            user.Location = location;
            user.Phone = phone;
            user.Timezone = timezone;
            _db.Save(user);
            _logger.Log("Profile updated");
        }

        public void DeleteUser(string userId)
        {
            var user = _db.GetUser(userId);
            _db.Delete(user);
            _email.Send(user.Email, "Your account has been deleted");
            _logger.Log("User deleted");
        }

        public void GenerateReport(string reportType, int year, int month, int day, bool includeArchived)
        {
            if (year < 1900 || year > 2100)
            {
                return;
            }

            var data = _db.GetUsersByDate(year, month, day);
            var report = new List<object>();

            foreach (var item in data)
            {
                report.Add(item);
            }

            System.IO.File.WriteAllText("/tmp/report.txt", String.Join(Environment.NewLine, report));
            _logger.Log("Report generated: " + reportType);
        }

        public void AnalyzeUserBehavior(string userId)
        {
            var user = _db.GetUser(userId);
            var loginCount = _db.GetLoginCount(userId);
            var lastLogin = _db.GetLastLogin(userId);

            if (loginCount > 100)
            {
                _logger.Log("High activity user");
            }
            else if (loginCount > 50)
            {
                _logger.Log("Medium activity user");
            }
            else if (loginCount > 10)
            {
                _logger.Log("Low activity user");
            }
            else
            {
                _logger.Log("Inactive user");
            }
        }

        public string ValidateEmail(string email)
        {
            String EmailDomain = email.Split('@')[1];
            return EmailDomain;
        }

        public void SendBulkEmail(List<string> recipients, string subject, string body)
        {
            foreach (var recipient in recipients)
            {
                _email.Send(recipient, subject);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    public interface IUserManagement
    {
        void CreateUser(string name);
        void UpdateUser(string id, string name);
        void DeleteUser(string id);
        string GetUser(string id);
        List<string> GetAllUsers();
        void SendEmail(string id, string message);
        void GenerateReport(string type);
        void AnalyzeBehavior(string id);
        void ValidateEmail(string email);
        void CreateBackup();
        void RestoreBackup();
    }

    // ❌ VIOLATION: Naming convention - Interface doesn't start with I
    public interface DatabaseConnection
    {
        void Connect();
    }

    // ❌ VIOLATION: Naming convention - Class starts with lowercase
    public class sqlDatabase
    {
        public void ExecuteQuery(string query) { }
        public object GetUser(string id) { return null; }
        public int GetLoginCount(string userId) { return 0; }
        public object GetLastLogin(string userId) { return null; }
        public List<object> GetUsersByDate(int year, int month, int day) { return new List<object>(); }
        public void Save(object user) { }
        public void Delete(object user) { }
    }

    // ❌ VIOLATION: Naming convention - Private field doesn't start with underscore
    public class EmailService
    {
        private string smtpServer = "smtp.example.com";

        public void Send(string to, string message)
        {
            // Hardcoded SMTP configuration
        }
    }

    // ❌ VIOLATION: Naming convention - Constant should be UPPER_CASE
    public class Logger
    {
        private const string logPath = "/var/logs/app.log";

        public void Log(string message)
        {
            System.IO.File.AppendAllText(logPath, message + Environment.NewLine);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // ❌ VIOLATION: DIP - Direct instantiation
            var userService = new UserService();

            userService.CreateUser(
                "John Doe",
                "john@example.com",
                "password123",
                "555-1234",
                "123 Main St",
                25,
                true,
                "admin"
            );

            userService.SendPasswordReset("user123");
            userService.UpdateUserProfile("user123", "Software Engineer", "avatar.jpg", "https://example.com", "TechCorp", "San Francisco", "555-5678", "PST");
            userService.GenerateReport("monthly", 2024, 2, 12, false);
            userService.AnalyzeUserBehavior("user123");

            Console.WriteLine("Test completed");
        }
    }
}
