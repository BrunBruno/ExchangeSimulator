using MediatR;

namespace ExchangeSimulator.Application.Requests.UserRequests.SetUserReview;
public class SetUserReviewRequest : IRequest {
    public int Review { get; set; }
}

