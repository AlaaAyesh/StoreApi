using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Entities;

namespace StoreCore.Dtos.Basket
{
    public class BasketDto : BaseEntity<int>
    {
        private List<BasketItem> _basketItems = new List<BasketItem>();
        public string BuyerId { get; set; }


    }
}
