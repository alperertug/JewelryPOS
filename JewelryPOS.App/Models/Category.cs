using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryPOS.App.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
