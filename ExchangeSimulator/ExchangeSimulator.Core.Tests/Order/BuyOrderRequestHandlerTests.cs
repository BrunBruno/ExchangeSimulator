using ExchangeSimulator.Application.Repositories;
using ExchangeSimulator.Application.Requests.OrderRequests.BuyOrder;
using ExchangeSimulator.Application.Services;
using ExchangeSimulator.Domain.Entities;
using ExchangeSimulator.Domain.Enums;
using ExchangeSimulator.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ExchangeSimulator.Core.Tests.Order;

public class BuyOrderRequestHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;

    public BuyOrderRequestHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockUserContextService = new Mock<IUserContextService>();
        _mockPlayerRepository = new Mock<IPlayerRepository>();
    }

    [Fact]
    public async Task Handle_Calls_Player_Repository_On_Success()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player
                {
                    LockedBalance = 100
                },
                Name = "TestCoin"
            }
        });
        _mockPlayerRepository.Setup(x => x.GetPlayerByUserIdAndGameName(request.GameName, userId)).ReturnsAsync(new Player
        {
            PlayerCoins = new()
            {
                new PlayerCoin
                {
                    Name = "TestCoin",
                    TotalBalance = 10
                }
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().NotThrowAsync();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Order_Was_Not_Found()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Order_Type_Is_Not_Buy()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Sell,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player(),
                Name = "TestCoin"
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Player_Is_Owner_Of_The_Order()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player
                {
                    UserId = userId
                },
                Name = "TestCoin"
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_NotFoundException_When_Player_Was_Not_Found()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player(),
                Name = "TestCoin"
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<NotFoundException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Order_Does_Not_Exist_In_Chosen_Game()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName1",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player(),
                Name = "TestCoin"
            }
        });
        _mockPlayerRepository.Setup(x => x.GetPlayerByUserIdAndGameName(request.GameName, userId)).ReturnsAsync(new Player
        {
            PlayerCoins = new()
            {
                new PlayerCoin
                {
                    Name = "TestCoin",
                    TotalBalance = 10
                }
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Game_Is_Not_Active()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Finished
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player(),
                Name = "TestCoin"
            }
        });
        _mockPlayerRepository.Setup(x => x.GetPlayerByUserIdAndGameName(request.GameName, userId)).ReturnsAsync(new Player
        {
            PlayerCoins = new()
            {
                new PlayerCoin
                {
                    Name = "TestCoin",
                    TotalBalance = 10
                }
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Order_Quantity_Is_Too_Low()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 4,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player(),
                Name = "TestCoin"
            }
        });
        _mockPlayerRepository.Setup(x => x.GetPlayerByUserIdAndGameName(request.GameName, userId)).ReturnsAsync(new Player
        {
            PlayerCoins = new()
            {
                new PlayerCoin
                {
                    Name = "TestCoin",
                    TotalBalance = 10
                }
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Player_LockedBalance_Is_Too_Low()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player
                {
                    LockedBalance = 10
                },
                Name = "TestCoin"
            }
        });
        _mockPlayerRepository.Setup(x => x.GetPlayerByUserIdAndGameName(request.GameName, userId)).ReturnsAsync(new Player
        {
            PlayerCoins = new()
            {
                new PlayerCoin
                {
                    Name = "TestCoin",
                    TotalBalance = 10
                }
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_BadRequestException_When_Player_Has_Not_Enough_Coins()
    {
        //given
        var userId = Guid.NewGuid();
        var request = new BuyOrderRequest
        {
            OrderId = Guid.NewGuid(),
            GameName = "GameName",
            Quantity = 5
        };

        _mockUserContextService.Setup(x => x.GetUserId()).Returns(userId);
        _mockOrderRepository.Setup(x => x.GetOrderById(request.OrderId)).ReturnsAsync(new Domain.Entities.Order
        {
            Type = OrderType.Buy,
            Quantity = 10,
            Price = 10,
            Game = new Domain.Entities.Game
            {
                Name = "GameName",
                Status = GameStatus.Active
            },
            PlayerCoin = new PlayerCoin
            {
                Player = new Player
                {
                    LockedBalance = 100
                },
                Name = "TestCoin"
            }
        });
        _mockPlayerRepository.Setup(x => x.GetPlayerByUserIdAndGameName(request.GameName, userId)).ReturnsAsync(new Player
        {
            PlayerCoins = new()
            {
                new PlayerCoin
                {
                    Name = "TestCoin",
                    TotalBalance = 4
                }
            }
        });

        //when
        var handler = new BuyOrderRequestHandler(_mockOrderRepository.Object, _mockUserContextService.Object, _mockPlayerRepository.Object, _mockTransactionRepository.Object);
        var act = () => handler.Handle(request, CancellationToken.None);

        //then
        await act.Should().ThrowAsync<BadRequestException>();

        _mockUserContextService.Verify(x => x.GetUserId(), Times.Once);
        _mockOrderRepository.Verify(x => x.GetOrderById(It.IsAny<Guid>()), Times.Once);
        _mockPlayerRepository.Verify(x => x.GetPlayerByUserIdAndGameName(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Order>()), Times.Never);
        _mockPlayerRepository.Verify(x => x.Update(It.IsAny<Player>()), Times.Never);
    }
}