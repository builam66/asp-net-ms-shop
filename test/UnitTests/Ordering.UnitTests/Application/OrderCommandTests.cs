using Moq;
using Ordering.Application.DTOs;
using Ordering.Application.Exceptions;
using Ordering.Application.Order.Commands.CancelOrder;
using Ordering.Application.Order.Commands.CreateOrder;
using Ordering.Application.Order.Commands.UpdateOrder;
using Ordering.Domain.Abstractions;
using Ordering.Domain.Enums;
using Ordering.Domain.Models.Order;
using Ordering.Domain.ValueObjects;

namespace Ordering.UnitTests.Application
{
    [TestFixture]
    public class OrderCommandTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CreateOrderCommandHandler _createOrderHandler;
        private UpdateOrderCommandHandler _updateOrderHandler;
        private CancelOrderCommandHandler _cancelOrderHandler;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepositoryMock.Setup(x => x.UnitOfWork).Returns(_unitOfWorkMock.Object);
            _createOrderHandler = new CreateOrderCommandHandler(_orderRepositoryMock.Object);
            _updateOrderHandler = new UpdateOrderCommandHandler(_orderRepositoryMock.Object);
            _cancelOrderHandler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ValidCommand_ShouldCreateOrder()
        {
            // Arrange
            var orderDto = new OrderDto
            (
                Id: Guid.NewGuid(),
                CustomerId: Guid.NewGuid(),
                OrderName: "TestOrder",
                ShippingAddress: new AddressDto("Test", "User", "testuser@example.com", "Duong 123", "VN", "HCM", "12345"),
                BillingAddress: new AddressDto("Test", "User", "testuser@example.com", "Duong 123", "VN", "HCM", "12345"),
                Payment: new PaymentDto("Test User", "1234567890123456", "12/30", "123", 1),
                Status: OrderStatus.Pending,
                OrderItems: new List<OrderItemDto>
                {
                    new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(), 2, 50)
                }
            );
            var command = new CreateOrderCommand(orderDto);

            // Act
            var result = await _createOrderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<CreateOrderResult>());
            });
            _orderRepositoryMock.Verify(x => x.Add(It.IsAny<Order>()), Times.Once);
            _orderRepositoryMock.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_OrderExists_ShouldUpdateOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(OrderId.Of(orderId), CustomerId.Of(Guid.NewGuid()), "TestOrder",
                Address.Of("Test", "User", "testuser@example.com", "Duong 123", "VN", "HCM", "12345"),
                Address.Of("Test", "User", "testuser@example.com", "Duong 123", "VN", "HCM", "12345"),
                Payment.Of("Test User", "1234567890123456", "12/30", "123", 1));

            _orderRepositoryMock.Setup(x => x.GetAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var updatedOrderDto = new OrderDto
            (
                Id: orderId,
                CustomerId: order.CustomerId.Value,
                OrderName: "UpdatedOrder",
                ShippingAddress: new AddressDto("Test A", "User A", "testuser_a@example.com", "Duong 456", "VietNam", "HN", "67890"),
                BillingAddress: new AddressDto("Test A", "User A", "testuser_a@example.com", "Duong 456", "VietNam", "HN", "67890"),
                Payment: new PaymentDto("Test User A", "6543210987654321", "12/40", "456", 2),
                Status: OrderStatus.Completed,
                OrderItems: new List<OrderItemDto>
                {
                    new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(), 2, 50)
                }
            );
            var command = new UpdateOrderCommand(updatedOrderDto);

            // Act
            var result = await _updateOrderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(order.OrderName, Is.EqualTo("UpdatedOrder"));
                Assert.That(order.Status, Is.EqualTo(OrderStatus.Completed));
            });
            _orderRepositoryMock.Verify(x => x.Update(order), Times.Once);
            _orderRepositoryMock.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_OrderDoesNotExist_ShouldThrowOrderNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var updatedOrderDto = new OrderDto
            (
                Id: orderId,
                CustomerId: Guid.NewGuid(),
                OrderName: "UpdatedOrder",
                ShippingAddress: new AddressDto("Test A", "User A", "testuser_a@example.com", "Duong 456", "VietNam", "HN", "67890"),
                BillingAddress: new AddressDto("Test A", "User A", "testuser_a@example.com", "Duong 456", "VietNam", "HN", "67890"),
                Payment: new PaymentDto("Test User A", "6543210987654321", "12/40", "456", 2),
                Status: OrderStatus.Pending,
                OrderItems: new List<OrderItemDto>
                {
                    new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(), 2, 50)
                }
            );
            var command = new UpdateOrderCommand(updatedOrderDto);

            _orderRepositoryMock.Setup(x => x.GetAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null!);

            // Act & Assert
            Assert.ThrowsAsync<OrderNotFoundException>(async () =>
                await _updateOrderHandler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task Handle_OrderExists_ShouldSetCancelledStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(OrderId.Of(orderId), CustomerId.Of(Guid.NewGuid()), "Test Order",
                Address.Of("Test", "User", "testuser@example.com", "Duong 123", "VN", "HCM", "12345"),
                Address.Of("Test", "User", "testuser@example.com", "Duong 123", "VN", "HCM", "12345"),
                Payment.Of("Test User", "1234567890123456", "12/30", "123", 1));

            _orderRepositoryMock.Setup(x => x.GetAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _cancelOrderHandler.Handle(new CancelOrderCommand(orderId), CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(order.Status, Is.EqualTo(OrderStatus.Cancelled));
            });
            _orderRepositoryMock.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
