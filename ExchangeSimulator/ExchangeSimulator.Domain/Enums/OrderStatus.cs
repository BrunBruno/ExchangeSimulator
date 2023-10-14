
namespace ExchangeSimulator.Domain.Enums;

/// <summary>
/// Status of ordeer
/// active - available for all users
/// freeze - not available for all users
/// </summary>
public enum OrderStatus {
    Active = 0,
    Freeze = 1
}