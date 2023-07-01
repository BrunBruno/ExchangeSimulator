namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// User entity.
/// </summary>
public class User
{
    /// <summary>
    /// Id.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Email address.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Username.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// Hashed password.
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Url with user image.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// User role.
    /// </summary>
    public Role Role { get; private set; }

    /// <summary>
    /// Role id.
    /// </summary>
    public int RoleId { get; private set; }
}