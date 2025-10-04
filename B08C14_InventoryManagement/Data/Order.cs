using System.ComponentModel.DataAnnotations;

namespace B08C14_InventoryManagement.Data
{
  public  enum OrderStatus
    {
        Pending=1,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
    public class Order:BaseEntity
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}