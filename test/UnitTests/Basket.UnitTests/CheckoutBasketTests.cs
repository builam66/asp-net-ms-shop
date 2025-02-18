using Basket.API.Basket.CheckoutBasket;
using Basket.API.Data;
using Basket.API.DTOs;
using Basket.API.Exceptions;
using Basket.API.Models;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using Moq;

namespace Basket.UnitTests;

[TestFixture]
public class CheckoutBasketTests
{
    private Mock<IBasketRepository> _basketRepositoryMock;
    private Mock<IPublishEndpoint> _publishEndpointMock;
    private CheckoutBasketCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new CheckoutBasketCommandHandler(_basketRepositoryMock.Object, _publishEndpointMock.Object);
    }

    [Test]
    public async Task Handle_BasketExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var basketCheckoutDto = new BasketCheckoutDTO { Username = "testuser" };
        var command = new CheckoutBasketCommand(basketCheckoutDto);
        var basket = new ShoppingCart("testuser")
        {
            Items =
            [
                new ShoppingCartItem { ProductId = Guid.NewGuid(), ProductName = "Product1", Price = 50, Quantity = 1, Color = "Red" },
                new ShoppingCartItem { ProductId = Guid.NewGuid(), ProductName = "Product2", Price = 50, Quantity = 1, Color = "Blue" }
            ]
        };

        _basketRepositoryMock.Setup(x => x.GetBasket("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);
        _publishEndpointMock.Setup(x => x.Publish(It.IsAny<BasketCheckoutEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _basketRepositoryMock.Setup(x => x.DeleteBasket("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _basketRepositoryMock.Verify(x => x.GetBasket("testuser", It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.Is<BasketCheckoutEvent>(e => e.TotalPrice == 100), It.IsAny<CancellationToken>()), Times.Once);
        _basketRepositoryMock.Verify(x => x.DeleteBasket("testuser", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_BasketDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        var basketCheckoutDto = new BasketCheckoutDTO { Username = "testuser" };
        var command = new CheckoutBasketCommand(basketCheckoutDto);
        var basketNotFoundException = new BasketNotFoundException("testuser");

        _basketRepositoryMock.Setup(x => x.GetBasket("testuser", It.IsAny<CancellationToken>()))
             .ReturnsAsync((ShoppingCart)null!);
        //_basketRepositoryMock.Setup(x => x.GetBasket("testuser", It.IsAny<CancellationToken>()))
        //    .ThrowsAsync(basketNotFoundException);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        _basketRepositoryMock.Verify(x => x.GetBasket("testuser", It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<BasketCheckoutEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _basketRepositoryMock.Verify(x => x.DeleteBasket("testuser", It.IsAny<CancellationToken>()), Times.Never);
    }
}
