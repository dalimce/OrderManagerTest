namespace Common
{
    public class OrderRequestModel
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string RequestCustomerId { get; set; }
    }

    public class OrderResponseModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsError { get; set; }
        public int Quantity { get; set; }
    }
}
