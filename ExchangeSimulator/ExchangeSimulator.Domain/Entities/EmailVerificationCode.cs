namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// Entity used for user email verification.
/// </summary>
public class EmailVerificationCode
{
    /// <summary>
    /// Code id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Code used for verifying user email.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Date when code expires.
    /// </summary>
    public DateTime ExpirationDate { get; set; }
}