using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Web.App.Models.Configuration;
using Web.App.Models.Email;

namespace Web.App.Helpers
{
    public class EmailSender
    {
        private EmailConfiguration _emailConfiguration = new EmailConfiguration();

        public EmailSender(IConfiguration configuration)
        {
            configuration.Bind("Email", _emailConfiguration);
        }

        #region Private Emailer file path
        private static readonly string emailTemplatePath = @"EmailTemplate/{0}.html";
        private static readonly string forgotPasswordEmailToUser = string.Format(emailTemplatePath, "ForgotPasswordToken");
        private static readonly string confirmEmailLinkToUser = string.Format(emailTemplatePath, "EmailConfirmation");
        #endregion

        #region Email subjects
        private static readonly string forgotPasswordEmailToUserSubject = "Reset your password";
        private static readonly string confirmEmailLinkToUserSubject = "Confirm your email address";

        #endregion

        #region Public methods

        public async Task SendAccountActivationConfirmationMailAsync(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = "hi this is test subject";
            //userEmailOptions.Resource = GetLinkedResource(userEmailOptions.ServerPath);
            userEmailOptions.Body = GetEmailBody("template path", userEmailOptions.PlaceHolders);

            //userEmailOptions.Attachments = new List<EmailAttachment>()
            //{
            //    new EmailAttachment()
            //    {
            //        FileName = "filename.ext",
            //        MediaType = "application/pdf",
            //        FileStream= File.OpenRead(Path.Combine(userEmailOptions.ServerPath, "path"))
            //    }
            //};
            await SendEmail(userEmailOptions);
        }

        public async Task SendForgotPasswordEmailToUserAsync(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = forgotPasswordEmailToUserSubject;
            userEmailOptions.Resource = GetLinkedResource(userEmailOptions.ServerPath);
            userEmailOptions.Body = GetEmailBody(forgotPasswordEmailToUser, userEmailOptions.PlaceHolders);

            await SendEmail(userEmailOptions);
        }

        public async Task SendConfirmationEmailLinkToUserAsync(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = confirmEmailLinkToUserSubject;
            userEmailOptions.Resource = GetLinkedResource(userEmailOptions.ServerPath);
            userEmailOptions.Body = GetEmailBody(confirmEmailLinkToUser, userEmailOptions.PlaceHolders);
            await SendEmail(userEmailOptions);
        }
        #endregion

        #region Private methods
        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mail = new MailMessage
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(_emailConfiguration.SenderAddress, _emailConfiguration.SenderDisplayName),
                IsBodyHtml = true
            };

            foreach (var toEmail in userEmailOptions.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(userEmailOptions.Body, null, MediaTypeNames.Text.Html);

            if (userEmailOptions.Resource != null)
            {
                alternateView.LinkedResources.Add(userEmailOptions.Resource);
                mail.AlternateViews.Add(alternateView);
            }

            if (userEmailOptions.Attachments != null
                && userEmailOptions.Attachments.Any())
            {
                foreach (var attachment in userEmailOptions.Attachments)
                {
                    var file = new Attachment(attachment.FileStream, attachment.FileName, attachment.MediaType);
                    mail.Attachments.Add(file);
                }
            }

            NetworkCredential networkCredential = new NetworkCredential(_emailConfiguration.UserName, _emailConfiguration.Password);

            SmtpClient smtp = new SmtpClient
            {
                Host = _emailConfiguration.Host,
                Port = _emailConfiguration.Port,
                EnableSsl = _emailConfiguration.EnableSsl,
                UseDefaultCredentials = _emailConfiguration.UseDefaultCredentials,
                Credentials = networkCredential
            };

            mail.BodyEncoding = Encoding.Default;

            await smtp.SendMailAsync(mail);
        }
        private LinkedResource GetLinkedResource(string serverPath)
        {
            //return new LinkedResource("wwwroot/logo.png", "image/png")
            //{
            //    ContentId = "logo"
            //};

            return null;
        }
        private string GetEmailBody(string filePath, List<KeyValuePair<string, string>> placeholders)
        {
            var body = File.ReadAllText(filePath, Encoding.UTF8);

            if (!string.IsNullOrEmpty(body) && placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    if (body.Contains(placeholder.Key))
                    {
                        body = body.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }

            return body;
        }
        #endregion

    }
}
