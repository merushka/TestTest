namespace WebApplication.DTOs
{
    public class ProductSalesRequest
    {
        public List<int> ProductIds { get; set; } = new();
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
