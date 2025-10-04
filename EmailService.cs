using System.Net.Mail;
using System.Net;
using System.IO;
using System.Text;

namespace I_Attend
{
    public class EmailService
    {
        public void SendEmailWithCsv(string senderEmail, string senderPassword, string recipientEmail,
        string subject, string body, string csvContent, string fileName = "Report.csv")
        {
            var smtp = new SmtpClient("smtp.gmail.com") // Or use user's mail provider
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var message = new MailMessage(senderEmail, recipientEmail, subject, body);
            message.IsBodyHtml = true;

            // Add CSV attachment
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            var stream = new MemoryStream(bytes);
            var attachment = new Attachment(stream, fileName, "text/csv");
            message.Attachments.Add(attachment);

            smtp.Send(message);
        }

    }
}
