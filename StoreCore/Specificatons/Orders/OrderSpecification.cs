using StoreCore.Entities.Order;

namespace StoreCore.Specificatons.Orders
{
    public class OrderSpecification:BaseSpecifications<Order,int>
    {
        public OrderSpecification(string buyerEmail)
            : base(o => o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.OrderItems);
            Includes.Add(o => o.DeliveryMethod);
        }
        public OrderSpecification(int id, string buyerEmail)
            : base(o => o.Id == id && o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.OrderItems);
            Includes.Add(o => o.DeliveryMethod);
        }

    }
}
