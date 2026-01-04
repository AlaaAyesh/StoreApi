namespace StoreCore.Entities
{
    public class Basket 
    {
        public int Id { get; set; }

        private List<BasketItem> _basketItems = new List<BasketItem>();
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();
        public string BuyerId { get; set; }
        public decimal SubTotal => _basketItems.Sum(item => item.Price * item.Quantity);
        public decimal Total => SubTotal; // Add logic for discounts, taxes, and shipping if needed
        public void AddItem(BasketItem item)
        {
            var existingItem = _basketItems.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _basketItems.Add(item);
            }
        }
        public void RemoveItem(int productId, int quantity)
        {
            var existingItem = _basketItems.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity -= quantity;
                if (existingItem.Quantity <= 0)
                {
                    _basketItems.Remove(existingItem);
                }
            }
        }
        public void ClearItems()
        {
            _basketItems.Clear();
        }
        
    }
}