using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;

namespace ExchangeSimulator.Application.Requests.UserRequests.GetUser;
public class GetUserRequestHandler : IRequestHandler<GetUserRequest, UserDto> {
    private readonly IUserContextService _userContext;
    private readonly IUserRepository _userRepository;

    public GetUserRequestHandler(IUserContextService userContext, IUserRepository userRepository) {
        _userContext = userContext;
        _userRepository = userRepository;
    }
    public async Task<UserDto> Handle(GetUserRequest request, CancellationToken cancellationToken) {
        var userId = _userContext.GetUserId()!.Value;

        var user = await _userRepository.GetUserById(userId) 
            ?? throw new NotFoundException("User not found");

        var userDto = new UserDto() {
            Email = user.Email,
            UserName = user.Username,
            ImageUrl = user.ImageUrl,
        };

        return userDto;
    }
}

