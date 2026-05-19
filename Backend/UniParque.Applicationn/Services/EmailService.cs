using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using UniParque.Application.Config;

namespace UniParque.Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;

    public EmailService(IOptions<EmailConfig> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

    public async Task SendEmailAsync(string toAddress, int verificationCode)
    {
        var issuerEmail = _emailConfig.Email;
        var appPasword = _emailConfig.AppPassword;
        var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587);
        client.Authenticate(issuerEmail, appPasword);

        var message = new MimeKit.MimeMessage();

        message.From.Add(new MailboxAddress("Verification", issuerEmail));

        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = "Verification Code for Password Change";

        message.Body = new TextPart("html")
        {
            Text = $"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <title>Password Verification</title>
    </head>
    <body style="font-family: Arial, sans-serif; background-color: #f7f7f7; margin:0; padding:0;">
        <table align="center" width="100%" cellpadding="0" cellspacing="0" style="max-width:600px; margin-top:40px; background-color:#ffffff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.1);">
            <tr>
                <td style="padding:20px; text-align:center; background-color:#0d6efd; color:#ffffff; border-top-left-radius:8px; border-top-right-radius:8px;">
                    <h2>Password Verification Code</h2>
                </td>
            </tr>
            <tr>
                <td style="padding:30px; text-align:center; color:#333333;">
                    <p style="font-size:16px;">Hello,</p>
                    <p style="font-size:16px;">Use the following verification code to complete your password change:</p>
                
                    <div style="margin:20px auto; display:inline-block; padding:15px 25px; background-color:#f1f1f1; border-radius:6px; font-size:24px; font-weight:bold; letter-spacing:2px; color:#0d6efd;">
                        {verificationCode}
                    </div>

                    <p style="font-size:14px; color:#666666; margin-top:20px;">
                        This code will expire in 10 minutes. <br/>
                        If you did not request a password change, please ignore this email.
                    </p>
                </td>
            </tr>
            <tr>
                <td style="padding:20px; text-align:center; font-size:12px; color:#999999;">
                    &copy; 2026 UniParque. All rights reserved.
                </td>
            </tr>
        </table>
    </body>
    </html>
    """
        };
        client.Send(message);
        client.Disconnect(true);
    }

    public async Task SendEmailMessageAsync(string fromAddress, string name, string messageText)
    {
        var issuerEmail = _emailConfig.Email;
        var appPassword = _emailConfig.AppPassword;

        var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587);
        await client.AuthenticateAsync(issuerEmail, appPassword);


        var adminMessage = new MimeMessage();

        adminMessage.From.Add(new MailboxAddress("UniParque System", issuerEmail));
        adminMessage.To.Add(MailboxAddress.Parse(issuerEmail));
        adminMessage.Subject = "📩 Yeni Contact Mesajı";

        adminMessage.Body = new TextPart("html")
        {
            Text = $"""
        <h2>Yeni mesaj gəldi 🚀</h2>
        <p><strong>Ad:</strong> {name}</p>
        <p><strong>Email:</strong> {fromAddress}</p>
        <p><strong>Mesaj:</strong></p>
        <p>{messageText}</p>
        """
        };

        await client.SendAsync(adminMessage);


        var userMessage = new MimeMessage();

        userMessage.From.Add(new MailboxAddress("UniParque Support", issuerEmail));
        userMessage.To.Add(MailboxAddress.Parse(fromAddress));
        userMessage.Subject = "Mesajınız bizə çatdı ✅";

        userMessage.Body = new TextPart("html")
        {
            Text = $"""
<!DOCTYPE html>
<html lang="az">
<head>
<meta charset="UTF-8">
<title>Mesaj qəbul edildi</title>
</head>

<body style="margin:0; padding:0; background-color:#f9fafb; font-family:Arial, sans-serif;">

<table align="center" width="100%" cellpadding="0" cellspacing="0" 
       style="max-width:600px; margin:40px auto; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);">

    <!-- HEADER -->
    <tr>
        <td style="padding:20px; text-align:center; background-color:#f97316; color:white;">
            <h2 style="margin:0;">UniParque</h2>
        </td>
    </tr>

    <!-- BODY -->
    <tr>
        <td style="padding:30px; text-align:left; color:#111827;">
            
            <h3 style="margin-bottom:15px;">Hörmətli {name},</h3>

            <p style="font-size:15px; color:#374151;">
                Mesajınız uğurla tərəfimizə çatdırıldı. 
                Komandamız tərəfindən ən qısa zamanda nəzərdən keçiriləcək.
            </p>

            <p style="font-size:15px; color:#374151; margin-top:15px;">
                Sizə tezliklə geri dönüş ediləcək. Səbriniz üçün təşəkkür edirik.
            </p>

            <!-- INFO BOX -->
            <div style="margin:25px 0; padding:20px; background-color:#f3f4f6; border-radius:10px;">
                <p style="margin:0 0 8px 0;"><strong>Ad:</strong> {name}</p>
                <p style="margin:0 0 8px 0;"><strong>Email:</strong> {fromAddress}</p>
            </div>

            <p style="font-size:14px; color:#6b7280;">
                Bu email avtomatik göndərilmişdir.
            </p>

        </td>
    </tr>

    <!-- FOOTER -->
    <tr>
        <td style="padding:20px; text-align:center; font-size:12px; color:#9ca3af;">
            © 2026 UniParque. All rights reserved.
        </td>
    </tr>

</table>

</body>
</html>
"""
        };

        await client.SendAsync(userMessage);

        await client.DisconnectAsync(true);
    }
}
