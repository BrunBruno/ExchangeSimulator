﻿using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulator.Application.Requests.GameRequests.CreateGame;

/// <summary>
/// Handler for creating new game
/// Creates starting coins
/// </summary>
public class CreateGameRequestHandler : IRequestHandler<CreateGameRequest>
{
    private readonly IUserContextService _userContextService;
    private readonly IGameRepository _gameRepository;
    private readonly IPasswordHasher<Game> _passwordHasher;
    private readonly IUserRepository _userRepository;

    public CreateGameRequestHandler(IUserContextService userContextService,
        IGameRepository gameRepository,
        IPasswordHasher<Game> passwordHasher,
        IUserRepository userRepository)
    {
        _userContextService = userContextService;
        _gameRepository = gameRepository;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }
    public async Task Handle(CreateGameRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId()!.Value;

        var user = await _userRepository.GetUserById(userId)
            ?? throw new NotFoundException("User not found.");


        var existingGame = await _gameRepository.GetGameByName(request.Name);

        if (existingGame is not null) {
            throw new BadRequestException("Game already exists.");
        }

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Money = request.Money,
            EndGame = request.EndGame,
            NumberOfPlayers = request.NumberOfPlayers,
            OwnerId = userId
        };

        if (game.EndGame < game.CreatedAt)
        {
            throw new BadRequestException("End date is before created date.");
        }

        game.PasswordHash = _passwordHasher.HashPassword(game, request.Password);

        game.StartingCoins = request.Coins.Select(coin => new StartingCoin
        {
            Id = Guid.NewGuid(),
            Name = coin.Name,
            Quantity = coin.Quantity,
            GameId = game.Id
        }).ToList();

        await _gameRepository.CreateGame(game);

        user.Games.Add(game);
        await _userRepository.Update(user);
    }
}