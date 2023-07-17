using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.UserRequests.SetUserReview;

public class SetUserReviewRequestHandler : IRequestHandler<SetUserReviewRequest> {
    private readonly IUserContextService _userContext;
    private readonly IUserRepository _userRepository;

    public SetUserReviewRequestHandler(IUserContextService userContext, IUserRepository userRepository) {
        _userContext = userContext;
        _userRepository = userRepository;
    }
    public async Task Handle(SetUserReviewRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;

        var user = await _userRepository.GetUserById(userId)
            ?? throw new NotFoundException("User not found.");

        user.Review = request.Review;
        await _userRepository.Update(user);
    }
}

