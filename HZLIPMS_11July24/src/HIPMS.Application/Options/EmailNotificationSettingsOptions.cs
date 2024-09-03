namespace HIPMS.Options;

public class EmailNotificationSettingsOptions
{
    public const string EmailNotificationSettings = "MailSettings"; /** String must match property in appsettings.json file */
    //public string Server { get; set; } = "smtp.office365.com";
    //public string Port { get; set; } = "587";
    //public string SenderName { get; set; } = "HOITSPS";
    //public string SenderEmail { get; set; } = "Support.PS@vedanta.co.in";
    //public string UserName { get; set; } = "Support PS (highbartech)";
    //public string Password { get; set; } = "z@qF2B6c";
    public string Server { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } 
}
public class SAPSettingsOptions
{
    public const string SAPSettings = "SAPSettings"; /** String must match property in appsettings.json file */
    public string Authkey { get; set; }
    public string SAPPOUrl { get; set; }
    public string SAPICUrl { get; set; }
    public string SAPRFIUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FilePath { get; set; }
    
}