// Path: StoreService/Service/Payment/PaymobClient.cs
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreCore.Dtos.Orders;

namespace StoreService.Service.Payment
{
    public class PaymobClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymobClient> _logger;

        public string IframeId => _config["Paymob:IframeId"];
        public string IntegrationId => _config["Paymob:IntegrationId"];
        public string IntegrationIdWallet => _config["Paymob:IntegrationIdWallet"];

        public PaymobClient(HttpClient httpClient, IConfiguration config, ILogger<PaymobClient> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<string> CreatePaymentAsync(decimal amount, AddressOrderDto billing)
        {
            var apiKey = _config["Paymob:ApiKey"];
            var integrationId = _config["Paymob:IntegrationId"];

            var auth = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new { api_key = apiKey });
            var authData = await auth.Content.ReadFromJsonAsync<AuthResponse>();
            _logger.LogDebug("Paymob auth: {@authData}", authData);

            var order = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
            {
                auth_token = authData?.token,
                delivery_needed = false,
                amount_cents = (int)(amount * 100),
                currency = "EGP"
            });
            var orderData = await order.Content.ReadFromJsonAsync<OrderResponse>();
            _logger.LogDebug("Paymob order: {@orderData}", orderData);

            var paymentKey = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
            {
                auth_token = authData?.token,
                amount_cents = (int)(amount * 100),
                currency = "EGP",
                order_id = orderData?.id,
                integration_id = integrationId,
                billing_data = new
                {
                    first_name = billing?.FirstName ?? "Customer",
                    last_name = billing?.LastName ?? "Unknown",
                    email = "customer@example.com",
                    phone_number = "+201000000000",
                    street = billing?.Street ?? "Unknown",
                    city = billing?.City ?? "Cairo",
                    state = billing?.State ?? "Cairo",
                    country = billing?.Country ?? "EG"
                }
            });

            var keyData = await paymentKey.Content.ReadFromJsonAsync<PaymentKeyResponse>();
            _logger.LogDebug("Paymob payment key: {@keyData}", keyData);

            if (keyData == null || string.IsNullOrEmpty(keyData.token))
                throw new System.Exception("Failed to create Paymob payment key (card).");

            return keyData.token;
        }

        public async Task<WalletPaymentResult> CreateWalletPaymentAsync(decimal amount, AddressOrderDto billing, string walletNumber)
        {
            _logger.LogInformation("CreateWalletPaymentAsync called amount={Amount} wallet={Wallet}", amount, walletNumber);
            if (string.IsNullOrWhiteSpace(walletNumber))
                throw new System.ArgumentException("walletNumber is required for wallet payments.");

            var apiKey = _config["Paymob:ApiKey"];
            var integrationIdWallet = _config["Paymob:IntegrationIdWallet"];

            var auth = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new { api_key = apiKey });
            var authData = await auth.Content.ReadFromJsonAsync<AuthResponse>();
            _logger.LogDebug("Paymob auth: {@authData}", authData);

            var order = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
            {
                auth_token = authData?.token,
                delivery_needed = false,
                amount_cents = (int)(amount * 100),
                currency = "EGP"
            });
            var orderData = await order.Content.ReadFromJsonAsync<OrderResponse>();
            _logger.LogDebug("Paymob order: {@orderData}", orderData);

            var paymentKey = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
            {
                auth_token = authData?.token,
                amount_cents = (int)(amount * 100),
                currency = "EGP",
                order_id = orderData?.id,
                integration_id = integrationIdWallet,
                billing_data = new
                {
                    first_name = billing?.FirstName ?? "Customer",
                    last_name = billing?.LastName ?? "Unknown",
                    email = "wallet@user.com",
                    phone_number = walletNumber,
                    street = billing?.Street ?? "Unknown",
                    city = billing?.City ?? "Cairo",
                    state = billing?.State ?? "Cairo",
                    country = billing?.Country ?? "EG"
                }
            });

            var keyData = await paymentKey.Content.ReadFromJsonAsync<PaymentKeyResponse>();
            _logger.LogDebug("Paymob payment key (wallet): {@keyData}", keyData);

            if (keyData == null || string.IsNullOrEmpty(keyData.token))
                throw new System.Exception("Failed to create Paymob payment key (wallet).");

            var wallet = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payments/pay", new
            {
                source = new { identifier = walletNumber, subtype = "WALLET" },
                payment_token = keyData.token
            });

            var walletResponse = await wallet.Content.ReadFromJsonAsync<WalletResponse>();
            _logger.LogDebug("Paymob wallet response: {@walletResponse}", walletResponse);

            if (walletResponse == null)
                throw new System.Exception("Paymob wallet response is null.");

            if (string.IsNullOrEmpty(walletResponse.redirect_url))
                throw new System.Exception($"Paymob wallet response missing redirect_url. Full response: {System.Text.Json.JsonSerializer.Serialize(walletResponse)}");

            return new WalletPaymentResult
            {
                PaymentToken = keyData.token,
                RedirectUrl = walletResponse.redirect_url
            };
        }

        private class AuthResponse { public string token { get; set; } = string.Empty; }
        private class OrderResponse { public int id { get; set; } }
        private class PaymentKeyResponse { public string token { get; set; } = string.Empty; }
        private class WalletResponse { public string redirect_url { get; set; } = string.Empty; }

        public class WalletPaymentResult
        {
            public string PaymentToken { get; set; } = string.Empty;
            public string RedirectUrl { get; set; } = string.Empty;
        }
    }
}
