using System;
using Microsoft.Extensions.DependencyInjection;
using StoreCore.Entities.Payments;
using StoreCore.ServicesContract;
using StoreService.Service.Payment.Strategies;

namespace StoreService.Service.Payment
{
    public interface IPaymentStrategyFactory
    {
        IPaymentStrategy GetStrategy(PaymentMethodType method);
    }

    public class PaymentStrategyFactory : IPaymentStrategyFactory
    {
        private readonly IServiceProvider _sp;

        public PaymentStrategyFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public IPaymentStrategy GetStrategy(PaymentMethodType method)
        {
            return method switch
            {
                PaymentMethodType.PaymobCard => _sp.GetRequiredService<PaymobCardPaymentStrategy>(),
                PaymentMethodType.PaymobWallet => _sp.GetRequiredService<PaymobWalletPaymentStrategy>(),
                PaymentMethodType.Cash => _sp.GetRequiredService<CashPaymentStrategy>(),
                _ => throw new NotImplementedException($"Payment method {method} not supported")
            };
        }
    }
}
