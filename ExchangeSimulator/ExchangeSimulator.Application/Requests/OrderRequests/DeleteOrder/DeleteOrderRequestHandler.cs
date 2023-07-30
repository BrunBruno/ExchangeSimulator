using MediatR;

namespace ExchangeSimulator.Application.Requests.OrderRequests.DeleteOrder;

public class DeleteOrderRequestHandler : IRequestHandler<DeleteOrderRequest>
{
    public Task Handle(DeleteOrderRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}