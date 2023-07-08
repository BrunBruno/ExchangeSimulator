namespace ExchangeSimulator.Application.Requests.IsEmailVerified;

/// <summary>
/// Dto that return information if user's email is verified.
/// </summary>
public class IsEmailVerifiedDto
{
    /// <summary>
    /// Information if user's email is verified.
    /// </summary>
    public bool IsEmailVerified { get; set; }
}