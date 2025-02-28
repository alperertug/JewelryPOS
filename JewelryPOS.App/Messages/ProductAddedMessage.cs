using JewelryPOS.App.Models;

namespace JewelryPOS.App.Messages
{
    public class ProductAddedMessage
    {
        public Product NewProduct { get; }

        public ProductAddedMessage(Product newProduct)
        {
            NewProduct = newProduct;
        }
    }
}
