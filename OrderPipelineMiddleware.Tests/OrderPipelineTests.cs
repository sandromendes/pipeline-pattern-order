using Moq;
using OrderPipelineMiddleware.Domain;
using OrderPipelineMiddleware.Middleware;
using OrderPipelineMiddleware.Pipeline;
using OrderPipelineMiddleware.Services.Interfaces;
using OrderPipelineMiddleware.Steps;

namespace OrderPipelineMiddleware.Tests
{
    public class OrderPipelineTests
    {
        /// <summary>
        /// Given an order with valid data, 
        /// When the ValidateOrderStep is executed, 
        /// Then it should not throw an exception and return the same order.
        /// </summary>
        [Fact]
        public async Task ValidateOrderStep_Should_Validate_ValidOrder()
        {
            // Given
            var order = new Order
            {
                CustomerId = "123",
                Items = new List<OrderItem> { new OrderItem { ProductId = "P1", Quantity = 1 } }
            };
            var step = new ValidateOrderStep();

            // When
            var result = await step.ProcessAsync(order);

            // Then
            Assert.Equal(order, result);
        }

        /// <summary>
        /// Given an order, 
        /// When the CalculateShippingStep is executed, 
        /// Then it should calculate and set the shipping cost.
        /// </summary>
        [Fact]
        public async Task CalculateShippingStep_Should_Calculate_ShippingCost()
        {
            // Given
            var order = new Order { TotalAmount = 100.0 };
            var step = new CalculateShippingStep();

            // When
            var result = await step.ProcessAsync(order);

            // Then
            Assert.Equal(15.0, result.ShippingCost);
        }

        /// <summary>
        /// Given an order with items, 
        /// When the ReserveStockStep is executed, 
        /// Then it should set each item's Reserved flag to true.
        /// </summary>
        [Fact]
        public async Task ReserveStockStep_Should_Reserve_Stock()
        {
            // Given
            var order = new Order
            {
                Items = new List<OrderItem>
        {
            new OrderItem { ProductId = "P1", Quantity = 1, Reserved = false },
            new OrderItem { ProductId = "P2", Quantity = 2, Reserved = false }
        }
            };
            var step = new ReserveStockStep();

            // When
            var result = await step.ProcessAsync(order);

            // Then
            Assert.All(result.Items, item => Assert.True(item.Reserved));
        }

        /// <summary>
        /// Given an order with a total amount and a working payment service, 
        /// When the ProcessPaymentStep is executed, 
        /// Then it should set the PaymentProcessed flag to true.
        /// </summary>
        [Fact]
        public async Task ProcessPaymentStep_Should_Process_Payment()
        {
            // Given
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock
                .Setup(ps => ps.ProcessPaymentAsync(It.IsAny<double>()))
                .ReturnsAsync(true);

            var order = new Order { TotalAmount = 200.0 };
            var step = new ProcessPaymentStep(paymentServiceMock.Object);

            // When
            var result = await step.ProcessAsync(order);

            // Then
            Assert.True(result.PaymentProcessed);
            paymentServiceMock.Verify(ps => ps.ProcessPaymentAsync(order.TotalAmount), Times.Once);
        }

        /// <summary>
        /// Given an order and a working invoice service, 
        /// When the GenerateInvoiceStep is executed, 
        /// Then it should generate an invoice and set the InvoiceGenerated flag to true.
        /// </summary>
        [Fact]
        public async Task GenerateInvoiceStep_Should_Generate_Invoice()
        {
            // Given
            var invoice = new Invoice { InvoiceId = "INV123", Amount = 200.0 };
            var invoiceServiceMock = new Mock<IInvoiceService>();
            invoiceServiceMock
                .Setup(a => a.GenerateInvoiceAsync(It.IsAny<Order>()))
                .ReturnsAsync(invoice);

            var order = new Order { TotalAmount = 200.0 };
            var step = new GenerateInvoiceStep(invoiceServiceMock.Object);

            // When
            var result = await step.ProcessAsync(order);

            // Then
            Assert.True(result.InvoiceGenerated);
            Assert.Equal(invoice, result.Invoice);
            invoiceServiceMock.Verify(a => a.GenerateInvoiceAsync(order), Times.Once);
        }

        /// <summary>
        /// Given an order with a customer ID and a working notification service, 
        /// When the NotifyCustomerStep is executed, 
        /// Then it should notify the customer and set the CustomerNotified flag to true.
        /// </summary>
        [Fact]
        public async Task NotifyCustomerStep_Should_Notify_Customer()
        {
            // Given
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock
                .Setup(ns => ns.NotifyCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var order = new Order { CustomerId = "123", OrderId = "ORDER1" };
            var step = new NotifyCustomerStep(notificationServiceMock.Object);

            // When
            var result = await step.ProcessAsync(order);

            // Then
            Assert.True(result.CustomerNotified);
            notificationServiceMock.Verify(
                ns => ns.NotifyCustomerAsync(order.CustomerId, It.Is<string>(msg => msg.Contains("ORDER1"))),
                Times.Once
            );
        }

        /// <summary>
        /// Given a complete pipeline and a valid order, 
        /// When the pipeline is executed, 
        /// Then all steps should be processed sequentially without errors.
        /// </summary>
        [Fact]
        public async Task Pipeline_Should_Process_AllSteps_Sequentially()
        {
            // Given
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(ps => ps.ProcessPaymentAsync(It.IsAny<double>())).ReturnsAsync(true);

            var invoiceServiceMock = new Mock<IInvoiceService>();
            invoiceServiceMock.Setup(a => a.GenerateInvoiceAsync(It.IsAny<Order>()))
                              .ReturnsAsync(new Invoice { InvoiceId = "INV123", Amount = 200.0 });

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(ns => ns.NotifyCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
                                   .ReturnsAsync(true);

            var order = new Order
            {
                CustomerId = "123",
                Items = new List<OrderItem> { new OrderItem { ProductId = "P1", Quantity = 1 } },
                TotalAmount = 200.0
            };

            var pipeline = await PipelineBuilder<Order>
                .Create()
                .UseMiddleware(ExceptionHandlingMiddleware<Order>.Apply)
                .AddStep(new ValidateOrderStep())
                .AddStep(new CalculateShippingStep())
                .AddStep(new ReserveStockStep())
                .AddStep(new ProcessPaymentStep(paymentServiceMock.Object))
                .AddStep(new GenerateInvoiceStep(invoiceServiceMock.Object))
                .AddStep(new NotifyCustomerStep(notificationServiceMock.Object))
                .ProcessAsync(order);

            // Then
            Assert.True(order.PaymentProcessed);
            Assert.True(order.InvoiceGenerated);
            Assert.NotNull(order.Invoice);
            Assert.True(order.CustomerNotified);
        }

    }
}