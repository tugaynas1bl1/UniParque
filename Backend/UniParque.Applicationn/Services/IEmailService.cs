namespace UniParque.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toAddress, int verificationCode);
    Task SendEmailMessageAsync(string fromAddress, string name, string messageText);
}
