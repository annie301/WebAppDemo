using WebAppDemo.Models;

namespace WebAppDemo.Services.EmailSender
{
    public interface IMailService
    {
        string CreateEmailBody(Booking booking);
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
