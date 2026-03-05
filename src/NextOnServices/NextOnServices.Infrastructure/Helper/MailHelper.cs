using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Helper;

public static class MailHelper
{
    public static bool SendEmail(string subject, string body, string emailTo)
    {
        try
        {
            using var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            // IMPORTANT: set this before Credentials
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(
                "community@globalresearchpanels.com",
                "YOUR_16_CHAR_APP_PASSWORD" // <-- App Password (not your normal password)
            );

            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("community@globalresearchpanels.com");
            mailMessage.To.Add(emailTo);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
            return true;


            //var smtpClient = new SmtpClient("smtp.gmail.com")
            //{
            //    Port = 587,                
            //    Credentials = new NetworkCredential("community@globalresearchpanels.com", "nexton@1234"),
            //    EnableSsl = true,
            //};
            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress("community@globalresearchpanels.com"),
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true,
            //};
            //mailMessage.To.Add(emailTo);
            //smtpClient.Send(mailMessage);
            //return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
