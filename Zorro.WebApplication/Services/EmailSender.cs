using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebPWrecover.Services;

//class used for sending an email for verifiying user
public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    //DI for email sender 
    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    //sends an email to subject
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var SendGridKey = "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";
        if (string.IsNullOrEmpty(SendGridKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(SendGridKey, subject, message, toEmail);
    }

    //execute method to send email to an email address inputed from user
    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        //creates the method
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("s3748814@student.rmit.edu.au", "Zorro Wallet"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };

        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        //send the email to the user
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }
}