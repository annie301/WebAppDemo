using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;
using WebAppDemo.Models;

namespace WebAppDemo.Services.EmailSending
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public string CreateEmailBody(Booking booking)
        {
            StringBuilder builder = new StringBuilder("Hello " + booking.FirstName + ", <br><br>");
            builder.AppendLine("Thank you for booking an appointment with the following details: <br>");
            builder.AppendLine("First Name: " + booking.FirstName + "<br>");
            builder.AppendLine("Last Name: " + booking.LastName + "<br>");
            builder.AppendLine("Email Address: " + booking.Email + "<br>");
            builder.AppendLine("Contact Number: " + booking.ContactNumber + "<br>");
            builder.AppendLine("Address: " + booking.AddressLine1 + ", ");
            if (booking.AddressLine2 != null) {
                builder.Append(booking.AddressLine2 + ", ");
            }
            builder.Append(booking.City + ", " + booking.Postcode + "<br>");
            builder.AppendLine("Date: " + booking.BookingStartDate.ToShortDateString() + "<br>");
            builder.AppendLine("Time slot: " + booking.BookingStartDate.Hour + ":00-" + (booking.BookingStartDate.Hour +2) + ":00<br>");
            builder.AppendLine("Vehicle Registration: " + booking.VehicleRegNumber + "<br>");
            builder.AppendLine("Job Category: " + booking.JobCategory.ToString() + "<br>");
            if(booking.Comments != null)
            {
                builder.AppendLine("Comments: " + booking.Comments + "<br><br>");
            }
            builder.AppendLine("<br>If you need to make any changes, please reply to this email.");

            return builder.ToString();
        }
    }
}
