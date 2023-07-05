namespace ExchangeSimulator.Infrastructure.EF.Options;

/// <summary>
/// Settings used for Smtp service.
/// </summary>
public class SmtpOptions
{
    /// <summary>
    /// Application email.
    /// </summary>
    public string FromMail { get; set; }

    /// <summary>
    /// Application email password.
    /// </summary>
    public string FromPassword { get; set; }

    /// <summary>
    /// Html code for message.
    /// </summary>
    public string Body { get; set; }
}