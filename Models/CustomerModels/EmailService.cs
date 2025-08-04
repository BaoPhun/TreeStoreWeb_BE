using System.Net.Mail;
using System.Net;

namespace TreeStore.Models.CustomerModels
{
    public class EmailService
    { 
        private static readonly string SmtpHost = "smtp.gmail.com"; // Thay bằng host SMTP của bạn
        private static readonly int SmtpPort = 587; // Thay bằng port SMTP của bạn
        private static readonly string SmtpUser = "khanhdangfc2004@gmail.com"; // Thay bằng email của bạn
        private static readonly string SmtpPass = "bgep czzx llql dnim"; // Thay bằng mật khẩu email của bạn

        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(SmtpHost, SmtpPort)) // Đảm bảo gọi đúng SmtpPort
            {
                client.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(SmtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }

    }
}
