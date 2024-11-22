namespace OrderPipelineMiddleware.Domain
{
    public class Invoice
    {
        public string InvoiceId { get; set; }
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public DateTime GeneratedDate { get; set; }
    }
}
