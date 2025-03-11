using JewelryPOS.App.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryPOS.App.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        [Required]
        public int Stock { get; set; }

        public double? Weight { get; set; }

        [MaxLength(50)]
        public string? Karat { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public Currency Currency { get; set; } = Currency.TRY;

        [MaxLength(50)]
        public string? Barcode { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
