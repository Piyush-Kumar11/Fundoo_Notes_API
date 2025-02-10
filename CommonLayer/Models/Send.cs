using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonLayer.Models
{
    public class Send
    {
        public string SendMail(string ToEmail, string Token)
        {
            string FromEmail = "piyushkumar95459@gmail.com";
            MailMessage message = new MailMessage(FromEmail,ToEmail);
            string MailBody = "The Token for the reset password: " + Token;
            message.Subject = "Token generated for resetting password";
            message.Body = MailBody.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);
            NetworkCredential credential = new NetworkCredential(FromEmail, "neamrjpyxgodhsss");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = credential;

            smtpClient.Send(message);
            return ToEmail;
        }

        public string SendMailToCollaborator(string toEmail, string title, string description, DateTime createdAt)
        {
            try
            {
                string fromEmail = "piyushkumar95459@gmail.com";
                string fromPassword = "neamrjpyxgodhsss";

                string mailBody = $@"
            <html>
            <body>
                <h3>You have been added as a collaborator on a note!</h3>
                <p><b>Title:</b> {title}</p>
                <p><b>Description:</b> {description}</p>
                <p><b>Created At:</b> {createdAt}</p>
                <br/>
                <p>You can access this note in your FundooNotes app.</p>
            </body>
            </html>";

                MailMessage message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = "Collaboration Added!",
                    Body = mailBody,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, fromPassword)
                };

                smtpClient.Send(message);
                return $"Email sent successfully to {toEmail}!";
            }
            catch (Exception ex)
            {
                return $"Failed to send email: {ex.Message}";
            }
        }
    }
}
