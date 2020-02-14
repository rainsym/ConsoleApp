using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class SendGrid
    {
        public static async Task Execute()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("tanhoang@futurify.vn", "Rainsym");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("developer@futurify.vn", "Developer");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
