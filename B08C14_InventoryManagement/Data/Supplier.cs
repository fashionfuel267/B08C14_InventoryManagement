namespace B08C14_InventoryManagement.Data
{
    public class Supplier:BaseEntity
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Address { get; set; } = "";
    }
}