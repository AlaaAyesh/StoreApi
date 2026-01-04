using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCore.Dtos.Payments
{
    public class WalletPaymentResponseDto
    {
        public string PaymentToken { get; set; }
        public string RedirectUrl { get; set; }
    }
}
