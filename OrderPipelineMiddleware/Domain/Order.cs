namespace OrderPipelineMiddleware.Domain
{
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
        public double TotalAmount { get; set; }
        public double ShippingCost { get; set; }
        public bool PaymentProcessed { get; set; }
        public bool InvoiceGenerated { get; set; }
        public Invoice Invoice { get; set; }
        public bool CustomerNotified { get; set; }
    }
}
