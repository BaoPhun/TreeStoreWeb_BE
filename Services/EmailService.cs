using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TreeStore.Models.Entities;

namespace TreeStore.Services.Mail
{
    public class EmailService : IEmailService
    {
        public async Task SendOrderConfirmationEmail(string toEmail, Order order)
        {
            var subject = "Xác nhận đơn hàng từ TreeStore";

            // Lấy email của khách hàng từ đối tượng order
            var customerEmail = order.Customer?.Email ?? "Không có email khách hàng"; // Lấy email từ đây

            var body = $@"
        <p>Xin chào <strong>{order.Customer?.FullName ?? "Khách hàng"}</strong>,</p>
        <p>Cảm ơn bạn đã đặt hàng tại <strong>TreeStore</strong>.</p>
        <p>Email của bạn: <strong>{customerEmail}</strong></p> 
        <p><strong>Mã đơn hàng:</strong> #{order.OrderId}</p>
        <p><strong>Ngày đặt:</strong> {order.CreateOn:dd/MM/yyyy HH:mm}</p>
        <p><strong>Tổng tiền:</strong> {order.TotalAmount:N0} VND</p>
        <p><strong>Địa chỉ:</strong> {order.Customer?.Address}</p>
        <br/>
        <p>Chúng tôi sẽ xử lý đơn hàng sớm nhất. Cảm ơn bạn!</p>";

            // Email gửi đi là email cố định của hệ thống
            var senderEmailAddress = "nguyennhatduy00002@gmail.com"; 
            var senderAppPassword = "gkoy geaa mgnt gryk"; 

            var message = new MailMessage
            {
                From = new MailAddress(senderEmailAddress, "TreeStore"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail); // Email người nhận vẫn là email của khách hàng

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmailAddress, senderAppPassword)
            };

            await smtp.SendMailAsync(message);
        }
    }
}
