using HIPMS.Options;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
//using static Abp.Net.Mail.EmailSettingNames;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace HIPMS.Services.SendEmail;
public class SendEmailOptions
{
    public string Subject { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public string Addresslist { get; set; } = string.Empty;
    public string ToName { get; set; }
    public string BCC { get; set; }
    public string CC { get; set; }
    public bool IsHTML { get; set; }
    public string Body { get; set; }
    public bool IsNoReply { get; set; }
    public List<string> Attachments { get; set; }
    public Attachment Attachment { get; set; }
}

public class SendEmail : ISendEmail
{
    private readonly ILogger<SendEmail> _logger;
    //private readonly SmtpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmailNotificationSettingsOptions _emailNotificationSettings;


    static bool mailSent = false;
    public SendEmail(
        ILogger<SendEmail> logger,
        IHttpContextAccessor httpContextAccessor,
        IOptions<EmailNotificationSettingsOptions> emailNotificationSettings)
    {
        //_client = new SendGridClient(emailNotificationSettings.Value.SendGridAPIKey);
        // _client = new SmtpClient(emailNotificationSettings.Value.SendGridAPIKey);
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _emailNotificationSettings = emailNotificationSettings.Value;

    }

    //Sendgrid
    //public async Task<bool> SendGrid_ExecuteAsync(SendEmailOptions options, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var emailNoReply = _emailNotificationSettings.EmailNoReply;
    //        var from = new EmailAddress(options.FromEmail ?? emailNoReply, options.FromName ?? "Radefy");
    //        var subject = options.Subject;
    //        var htmlContent = PrepareBody(options);
    //        string[] addresses = options.Addresslist.Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    //        var to = from;
    //        if (addresses.Length > 0)
    //        {
    //            to = new EmailAddress(addresses[0], options.ToName);
    //        }

    //        var objMM = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);

    //        var emails = new List<EmailAddress>();

    //        for (int i = 1; i < addresses.Length; i++)
    //        {
    //            emails.Add(new EmailAddress(addresses[i]));
    //        }
    //        if (emails.Count > 0)
    //        {
    //            objMM.AddTos(emails);
    //        }

    //        if (options.Attachments is not null)
    //        {
    //            foreach (string att in options.Attachments)
    //            {
    //                byte[] byteData = Encoding.ASCII.GetBytes(File.ReadAllText(att));
    //                objMM.AddAttachment(new Attachment
    //                {
    //                    Content = Convert.ToBase64String(byteData),
    //                    Filename = "Transcript.txt",
    //                    Type = "txt/plain",
    //                    Disposition = "attachment"
    //                });
    //            }
    //        }

    //        addresses = (options.CC ?? "").Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    //        emails.Clear();
    //        foreach (string address in addresses)
    //        {
    //            emails.Add(new EmailAddress(address));
    //        }
    //        if (emails.Count > 0)
    //        {
    //            objMM.AddCcs(emails);
    //        }

    //        addresses = (options.BCC ?? "").Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    //        emails.Clear();
    //        foreach (string address in addresses)
    //        {
    //            emails.Add(new EmailAddress(address));
    //        };
    //        if (emails.Count > 0)
    //        {
    //            objMM.AddBccs(emails);
    //        }

    //        /* for single Attachment */
    //        if (options.Attachment != null)
    //        {
    //            objMM.AddAttachment(options.Attachment);
    //        }

    //        var response = await _client.SendEmailAsync(objMM, cancellationToken);
    //        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
    //        {
    //            _logger.LogError($"from: {objMM.From.Email} fromName: {objMM.From.Name} CC : {options.CC} BCC: {options.BCC} To: {objMM.Personalizations[0].Tos[0].Email} ToName: {objMM.Personalizations[0].Tos[0].Name} response.StatusCode : {response.StatusCode}");
    //            return false;
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, "Error during send email.");
    //        return false;
    //    }
    //    return true;
    //}

    //SMTP 
    public async Task<bool> ExecuteAsync(SendEmailOptions options, CancellationToken cancellationToken)
    {
        //var emailNoReply = _emailNotificationSettings.EmailNoReply;
        //if(options == null)
        //{
        //    options=new SendEmailOptions();
        //    options.FromEmail = "Support.PS@vedanta.co.in";
        //    options.FromName = "Test";
        //    options.Subject= "Test";
        //    options.Addresslist = "honey.bapna@gmail.com";
        //    options.ToName= "Test";

        //}
        //var from = new MailAddress(options.FromEmail , options.FromName);
        //var subject = options.Subject;

        //string[] addresses = options.Addresslist.Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //var to = from;
        //if (addresses.Length > 0)
        //{
        //    to = new MailAddress(addresses[0], options.ToName);
        //}

        //SmtpClient client = new SmtpClient(_emailNotificationSettings.Server);

        //MailMessage message = new MailMessage(from, to);
        //message.Body = "This is a test email message sent by an application. ";
        //// Include some non-ASCII characters in body and subject.
        //string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
        //message.Body += Environment.NewLine + someArrows;
        //message.BodyEncoding = System.Text.Encoding.UTF8;
        //message.Subject = "test message 1" + someArrows;
        //message.SubjectEncoding = System.Text.Encoding.UTF8;
        //// Set the method that is called back when the send operation ends.
        //client.SendCompleted += new
        //SendCompletedEventHandler(SendCompletedCallback);
        //// The userState can be any object that allows your callback
        //// method to identify this send operation.
        //// For this example, the userToken is a string constant.
        //string userState = "test message1";
        //client.SendAsync(message, userState);
        //Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
        //string answer = Console.ReadLine();
        //// If the user canceled the send, and mail hasn't been sent yet,
        //// then cancel the pending operation.
        //if (answer.StartsWith("c") && mailSent == false)
        //{
        //    client.SendAsyncCancel();
        //}

        //// Clean up.
        //message.Dispose();
        //Console.WriteLine("Goodbye.");
        return true;
    }
    public async Task<bool> ExecuteSMTPAsync(EmailNotificationSettingsOptions options, CancellationToken cancellationToken)
    {

        //using (SmtpClient smtpClient = new SmtpClient())
        //{
        using (MimeMessage message = new MimeMessage())
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(options.SenderEmail));
                email.To.Add(MailboxAddress.Parse("honey.bapna@gmail.com"));
                email.Subject = "Test";
                email.Body = new TextPart(TextFormat.Html) { Text = "<h1>your message body</h1>" };
                var smtp = new SmtpClient();

                await smtp.ConnectAsync(options.Server, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(options.UserName, options.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                //Error, could not send the message
                // Do something
            }
        }
        //}




        //var emailNoReply = _emailNotificationSettings.EmailNoReply;

        //var from = new MailAddress(options.SenderEmail, options.SenderName);
        //var subject = "test";//options.Subject;

        ////string[] addresses = options.Addresslist.Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //var to = from;
        ////if (addresses.Length > 0)
        ////{
        //    to = new MailAddress("honey.bapna@gmail.com");
        ////}

        //SmtpClient client = new SmtpClient(_emailNotificationSettings.Server);
        //client.UseDefaultCredentials = false;
        //client.EnableSsl = true;
        //client.Credentials = new System.Net.NetworkCredential(options.UserName, options.Password);
        //client.Port = 25;
        //MailMessage message = new MailMessage(from, to);
        //message.Body = "This is a test email message sent by an application. ";

        //// Include some non-ASCII characters in body and subject.
        //string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
        //message.Body += Environment.NewLine + someArrows;
        //message.BodyEncoding = System.Text.Encoding.UTF8;
        //message.Subject = "test message 1" + someArrows;
        //message.SubjectEncoding = System.Text.Encoding.UTF8;
        //// Set the method that is called back when the send operation ends.
        //client.SendCompleted += new
        //SendCompletedEventHandler(SendCompletedCallback);
        //// The userState can be any object that allows your callback
        //// method to identify this send operation.
        //// For this example, the userToken is a string constant.
        //string userState = "test message1";

        //try
        //{
        //    //client.SendMailAsync(message);
        //    client.SendAsync(message, userState);
        //}
        //catch(Exception ex)
        //{

        //}


        //Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
        //string answer = Console.ReadLine();
        //// If the user canceled the send, and mail hasn't been sent yet,
        //// then cancel the pending operation.
        //if (answer.StartsWith("c") && mailSent == false)
        //{
        //    client.SendAsyncCancel();
        //}

        // Clean up.
        //message.Dispose();
        //Console.WriteLine("Goodbye.");
        return true;
    }
    public async Task<bool> ExecuteSMTPAsync(string addressesList, string subject, string body, CancellationToken cancellationToken)
    {
        if (_emailNotificationSettings.IsEnabled)
        {
            //using (SmtpClient smtpClient = new SmtpClient())
            //{
            using (MimeMessage message = new MimeMessage())
            {
                try
                {
                    string[] addresses = addressesList.Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //var to = _emailNotificationSettings.SenderEmail;
                    //if (addresses.Length > 0)
                    //{
                    //    for (int i = 1; i < addresses.Length; i++)
                    //    {
                    //        to = new MailAddress(addresses[i], addresses[i]).ToString();
                    //    }
                    //}
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(_emailNotificationSettings.SenderEmail));

                    for (int i = 0; i < addresses.Length; i++)
                    {
                        email.To.Add(MailboxAddress.Parse(addresses[i]));
                    }

                    //email.To.Add(MailboxAddress.Parse("honey.bapna@gmail.com"));
                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Html) { Text = "<h1>" + subject + "</h1><body>" + body + "</body>" };
                    var smtp = new SmtpClient();

                    await smtp.ConnectAsync(_emailNotificationSettings.Server, 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_emailNotificationSettings.UserName, _emailNotificationSettings.Password);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    //Error, could not send the message
                    // Do something
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //private string PrepareBody(SendEmailOptions options)
    //{
    //    const string noReplyMessage = "Please do not reply to this email as this email address is not monitored.";

    //    if (options.IsNoReply)
    //    {
    //        return options.IsHTML
    //            ? PrepareBodyHtml($"{options.Body}<br />{noReplyMessage}")
    //            : $"{options.Body} {noReplyMessage}";
    //    }

    //    return options.IsHTML ? PrepareBodyHtml(options.Body ?? "") : options.Body ?? "";
    //}

    //private string PrepareBodyHtml(string html)
    //{
    //    var doc = new HtmlDocument();
    //    doc.LoadHtml(html);
    //    var links = doc.DocumentNode.Descendants("a");
    //    foreach (var link in links)
    //    {
    //        string href = link.GetAttributeValue("href", "");
    //        if (!string.IsNullOrEmpty(href))
    //        {
    //            if (!href.StartsWith("http"))
    //            {
    //                var isSecureConnection = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false;
    //                if (_httpContextAccessor.HttpContext is not null)
    //                {
    //                    _logger.LogInformation("Original href: {href}", href);
    //                    href = href.ToAbsoluteUrl(_httpContextAccessor.HttpContext, isSecureConnection);
    //                }
    //                _logger.LogInformation("Absolute href built: {href}", href);
    //                link.Attributes["href"].Value = href;
    //            }
    //        }
    //    }
    //    return doc.DocumentNode.OuterHtml;
    //}


    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        // Get the unique identifier for this asynchronous operation.
        String token = (string)e.UserState;

        if (e.Cancelled)
        {
            Console.WriteLine("[{0}] Send canceled.", token);
        }
        if (e.Error != null)
        {
            Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
        }
        else
        {
            Console.WriteLine("Message sent.");
        }
        mailSent = true;
    }
}