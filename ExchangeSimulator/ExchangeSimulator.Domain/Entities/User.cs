namespace ExchangeSimulator.Domain.Entities;

/// <summary>
/// User entity.
/// </summary>
public class User
{
    /// <summary>
    /// Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Hashed password.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Url with user image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// User role.
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Role id.
    /// </summary>
    public int RoleId { get; set; } = (int)Roles.User;

    /// <summary>
    /// Is user email verified
    /// </summary>
    public bool IsVerified { get; set; } = false;
}