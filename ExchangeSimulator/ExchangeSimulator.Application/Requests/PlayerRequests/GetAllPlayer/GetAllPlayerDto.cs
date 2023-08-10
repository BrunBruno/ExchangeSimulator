
namespace ExchangeSimulator.Application.Requests.PlayerRequests.GetAllPlayer;
public class GetAllPlayerDto {
    public decimal Balance { get; set; }
    public decimal TurnOver { get; set; }

    public int TradesQuantity { get; set; }
    public int CreatedOrders { get; set; }
}

