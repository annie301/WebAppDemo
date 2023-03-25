using WebAppDemo.Models;

namespace WebAppDemo.Services.EmailSending
{
    public interface IMailService
    {
        string CreateEmailBody(Booking booking);
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
