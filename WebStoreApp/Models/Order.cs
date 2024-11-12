namespace WebStoreApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate {  get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending,      
        Processing,   
        Shipped,       
        Delivered,    
        Canceled,    
        Completed   
    }


}
