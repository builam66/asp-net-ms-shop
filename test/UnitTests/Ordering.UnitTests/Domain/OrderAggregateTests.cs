using Ordering.Domain.Enums;
using Ordering.Domain.Models.Order;
using Ordering.Domain.ValueObjects;

namespace Ordering.UnitTests.Domain
{
    [TestFixture]
    public class OrderAggregateTests
    {
        private OrderId _orderId;
        private CustomerId _customerId;
        private Address _address;
        private Payment _payment;

        [SetUp]
        public void Setup()
        {
            _orderId = OrderId.Of(Guid.NewGuid());
            _customerId = CustomerId.Of(Guid.NewGuid());
            _address = Address.Of("test", "user", "testuser@example.com", "Duong 123", "VN", "HCM", "12345");
            _payment = Payment.Of("Test User", "1234567890123456", "12/30", "123", 1);
        }

        [Test]
        public void Create_ValidParameters_ShouldReturnOrder()
        {
            // Act
            var order = Order.Create(_orderId, _customerId, "TestOrder", _address, _address, _payment);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(order, Is.Not.Null);
                Assert.That(order.Id, Is.EqualTo(_orderId));
                Assert.That(order.CustomerId, Is.EqualTo(_customerId));
                Assert.That(order.OrderName, Is.EqualTo("TestOrder"));
                Assert.That(order.ShippingAddress, Is.EqualTo(_address));
                Assert.That(order.BillingAddress, Is.EqualTo(_address));
                Assert.That(order.Payment, Is.EqualTo(_payment));
                Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));
                // Assert.AreEqual(1, order.DomainEvents.Count);
                // Assert.IsInstanceOf<OrderCreatedEvent>(order.DomainEvents.First());
            });
        }

        [Test]
        public void Update_ValidParameters_ShouldUpdateOrder()
        {
            // Arrange
            var order = Order.Create(_orderId, _customerId, "TestOrder", _address, _address, _payment);
            var newAddress = Address.Of("test_clone", "user_clone", "testuser_clone@example.com", "Duong 456", "VietNam", "HN", "67890");
            var newPayment = Payment.Of("Test User Clone", "1234567890123456", "12/40", "456", 2);

            // Act
            order.Update("UpdatedOrder", newAddress, newAddress, newPayment, OrderStatus.Completed);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(order.OrderName, Is.EqualTo("UpdatedOrder"));
                Assert.That(order.ShippingAddress, Is.EqualTo(newAddress));
                Assert.That(order.BillingAddress, Is.EqualTo(newAddress));
                Assert.That(order.Payment, Is.EqualTo(newPayment));
                Assert.That(order.Status, Is.EqualTo(OrderStatus.Completed));
                // Assert.AreEqual(2, order.DomainEvents.Count);
                // Assert.IsInstanceOf<OrderUpdatedEvent>(order.DomainEvents.Last());
            });
        }

        [Test]
        public void Add_ValidParameters_ShouldAddOrderItem()
        {
            // Arrange
            var order = Order.Create(_orderId, _customerId, "TestOrder", _address, _address, _payment);
            var productId = ProductId.Of(Guid.NewGuid());

            // Act
            order.Add(productId, 2, 50);

            // Assert
            var orderItem = order.OrderItems[0];
            Assert.Multiple(() =>
            {
                Assert.That(order.OrderItems, Has.Count.EqualTo(1));
                Assert.That(orderItem.ProductId, Is.EqualTo(productId));
                Assert.That(orderItem.Quantity, Is.EqualTo(2));
                Assert.That(orderItem.Price, Is.EqualTo(50));
            });
        }

        [Test]
        public void Remove_ValidProductId_ShouldRemoveOrderItem()
        {
            // Arrange
            var order = Order.Create(_orderId, _customerId, "TestOrder", _address, _address, _payment);
            var productId = ProductId.Of(Guid.NewGuid());
            order.Add(productId, 2, 50);

            // Act
            order.Remove(productId);

            // Assert
            Assert.That(order.OrderItems, Is.Empty);
        }

        [Test]
        public void SetCancelledStatus_ValidStatus_ShouldSetStatusToCancelled()
        {
            // Arrange
            var order = Order.Create(_orderId, _customerId, "TestOrder", _address, _address, _payment);

            // Act
            order.SetCancelledStatus();

            // Assert
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Cancelled));
        }
    }
}
