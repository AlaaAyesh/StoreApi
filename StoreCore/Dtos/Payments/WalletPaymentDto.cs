using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCore.Dtos.Orders;

namespace StoreCore.Dtos.Payments
{
    public class WalletPaymentDto
    {
        public string BasketId { get; set; } = string.Empty;
        public AddressOrderDto BillingAddress { get; set; } // can be null
        public string WalletNumber { get; set; } = string.Empty; // e.g. 01XXXXXXXXX
    }
}
