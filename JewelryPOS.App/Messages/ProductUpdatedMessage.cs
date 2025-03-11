using JewelryPOS.App.Models;

namespace JewelryPOS.App.Messages
{
    public class ProductUpdatedMessage
    {
        public Product UpdatedProduct { get; }
        public ProductUpdatedMessage(Product updatedProduct)
        {
            UpdatedProduct = updatedProduct;
        }
    }
}
