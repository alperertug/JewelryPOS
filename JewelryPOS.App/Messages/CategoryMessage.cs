using JewelryPOS.App.Models;

namespace JewelryPOS.App.Messages
{
    public class CategoryMessage
    {
        public Category Category { get; }
        public CategoryMessage(Category category)
        {
            Category = category;
        }
    }
}
