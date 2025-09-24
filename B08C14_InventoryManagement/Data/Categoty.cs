using System.ComponentModel.DataAnnotations;

namespace B08C14_InventoryManagement.Data
{
    public class Categoty:BaseEntity
    {
        [Display(Name="Category")]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product>? Products { get; set; }

    }
}