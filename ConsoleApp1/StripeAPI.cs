using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class StripeAPI
    {
        private static string secretkey = "sk_test_tOWvifRqdRPGgXsxF9Cbu2dj00MK2lJ587";

        public Customer GetCustomer(string id)
        {
            StripeConfiguration.ApiKey = secretkey;
            var customerService = new CustomerService();
            var customer = customerService.Get(id);
            Console.WriteLine($"Customer response: \r\n{JsonConvert.SerializeObject(customer)}");

            return customer;
        }

        public Customer CreateCustomer(string fullName, string email, string token)
        {
            StripeConfiguration.ApiKey = secretkey;
            var customerService = new CustomerService();
            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = email,
                Description = "Create new account",
                Source = token,
                Name = fullName
            });

            Console.WriteLine($"Customer response: \r\n{JsonConvert.SerializeObject(customer)}");

            return customer;
        }

        public string CreateCardTokens(string cardNumber, int expMonth, int expYear, string cvc)
        {
            StripeConfiguration.ApiKey = secretkey;

            var options = new TokenCreateOptions
            {
                Card = new CreditCardOptions
                {
                    Number = cardNumber,
                    ExpYear = expYear,
                    ExpMonth = expMonth,
                    Cvc = cvc
                }
            };

            var service = new TokenService();
            Token stripeToken = service.Create(options);

            Console.WriteLine($"Token response: \r\n{JsonConvert.SerializeObject(stripeToken)}");

            return stripeToken.Id;
        }

        public void Charge(string customerId, double amount, int bookingId)
        {
            StripeConfiguration.ApiKey = secretkey;

            // `source` is obtained with Stripe.js; see https://stripe.com/docs/payments/accept-a-payment-charges#web-create-token
            var options = new ChargeCreateOptions
            {
                Amount = long.Parse((amount * 100).ToString()),
                Currency = "usd",
                Description = $"Booking #{bookingId}",
                Customer = customerId,//"tok_1GBbPwBqhCJKs3Kh7bvRukv6",
                TransferGroup = $"Booking #{bookingId}"
            };
            var service = new ChargeService();
            var result = service.Create(options);
            Console.WriteLine($"Charge response: \r\n{JsonConvert.SerializeObject(result)}");
        }

        public void Refund(string chargeId, double? amount)
        {
            StripeConfiguration.ApiKey = secretkey;
            var options = new RefundCreateOptions
            {
                Amount = long.Parse((amount * 100).ToString()),
                Charge = chargeId,
                Reason = StripeRefundReasons.RequestedByCustomer,
                Metadata = new Dictionary<string, string>
                {
                    { "Description", $"Refund for charge ID #{chargeId}" }
                }
            };

            var refundService = new RefundService();
            var result = refundService.Create(options);

            Console.WriteLine($"Refund response: \r\n{JsonConvert.SerializeObject(result)}");
        }
    }

    public static class StripeRefundReasons
    {
        public const string Unknown = null;
        public const string Duplicate = "duplicate";
        public const string Fraudulent = "fraudulent";
        public const string RequestedByCustomer = "requested_by_customer";
    }
}
