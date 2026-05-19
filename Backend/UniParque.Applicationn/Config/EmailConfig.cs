namespace UniParque.Application.Config;

public class EmailConfig
{
    public string Section = "EmailSettings";

    public string Email {  get; set; } = string.Empty;
    public string AppPassword {  get; set; } = string.Empty;
}
