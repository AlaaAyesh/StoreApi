using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCore.Dtos.Payments
{
    public class PaymobWebhookDto
    {
        public string PaymentId { get; set; }
        public string Status { get; set; }
    }
}
