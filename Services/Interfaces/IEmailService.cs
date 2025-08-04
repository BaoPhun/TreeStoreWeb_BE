using TreeStore.Models.Entities;
using System.Threading.Tasks;

namespace TreeStore.Services.Mail
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmail(string toEmail, Order order);
    }
}
