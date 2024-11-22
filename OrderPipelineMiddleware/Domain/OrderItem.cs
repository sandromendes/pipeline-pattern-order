namespace OrderPipelineMiddleware.Domain
{
    public class OrderItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public bool Reserved { get; set; }
    }
}
