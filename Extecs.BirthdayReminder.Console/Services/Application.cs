using Extecs.BirthdayReminder.CLI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using System.Net.Mail;

namespace Extecs.BirthdayReminder.CLI.Services
{
    public interface IApplication
    {
        void Run();
    }
    public class Application : IApplication
    {
        private readonly ILogger<Application> _logger;

        // Path to your CSV file
        private const string _filePath = "cumples.csv";

        public Application(ILogger<Application> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Running Application!");

            _logger.LogInformation("Reading CSV file...!");
            var csvFile = this.ReadCsvFile(_filePath);

            var currentDate = DateTime.Now.Date;

            var birthdaysToday = csvFile.Where(person =>
                person.BirthDate.Month == currentDate.Month &&
                person.BirthDate.Day == currentDate.AddDays(1).Day);

            _logger.LogInformation("Looping through the birthday records...!");
            foreach (var item in birthdaysToday)
            {
                int years = DateTime.Now.Year - item.BirthDate.Year;
                _logger.LogInformation(item.FullName + " con NC "
                    + item.ControlNumber + " cumple "
                    + years + " mañana!");
                SendEmailNotification(item);

            }

            _logger.LogInformation("Application finished!");
        }

        private List<BirthdayRecord> ReadCsvFile(string filePath)
        {
            var returnData = new List<BirthdayRecord>();

            // Create an instance of TextFieldParser
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // Skip the header if your CSV file contains one
                if (!parser.EndOfData)
                {
                    parser.ReadLine();
                }

                // Read and process each line of the CSV file
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    // Process the fields as needed

                    int parsedControlNumberResult;
                    if (!int.TryParse(fields[1], out parsedControlNumberResult))
                    {
                        parsedControlNumberResult = 0;
                    }

                    returnData.Add(new BirthdayRecord
                    {
                        BirthDate = DateTime.Parse(fields[0].Trim()),
                        ControlNumber = parsedControlNumberResult,
                        FullName = fields[2].Trim()
                    });
                }
                return returnData;
            }
        }

        private void SendEmailNotification(BirthdayRecord birthdayRecord)
        {
            string senderEmail = "myhotmailaccount@hotmail.com";
            string password = "myPassword";

            // Recipient's email address
            string recipientEmail = "andeltoro@gmail.com";

            // SMTP server details
            string smtpServer = "smtp.office365.com";
            int smtpPort = 587; // Port may vary based on your email provider

            // Create an instance of the SmtpClient
            using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
            {
                // Enable SSL/TLS
                client.EnableSsl = true;

                // Set credentials
                client.Credentials = new NetworkCredential(senderEmail, password);

                // Create a MailMessage object
                MailMessage message = new MailMessage(senderEmail, recipientEmail);

                // Set subject and body
                message.Subject = "Birthday reminder about " + birthdayRecord.FullName;
                message.Body = birthdayRecord.FullName + " with Control Number " + birthdayRecord.ControlNumber
                    + " will be " + (DateTime.Now.Year - birthdayRecord.BirthDate.Year).ToString()
                    + " years tomorrow! ";

                try
                {
                    // Send the email
                    client.Send(message);
                    _logger.LogInformation("Email was successfully sent.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send email: {ex.Message}");
                }
            }
        }

    }
}
