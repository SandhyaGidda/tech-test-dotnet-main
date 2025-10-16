using ClearBank.DeveloperTest.Types;
using System;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentValidatorFactory
    {
        public IPaymentValidator GetValidator(PaymentScheme scheme)
        {
            return scheme switch
            {
                PaymentScheme.Bacs => new BacsPaymentValidator(),
                PaymentScheme.FasterPayments => new FasterPaymentValidator(),
                PaymentScheme.Chaps => new ChapsPaymentValidator(), 
                _ => throw new ArgumentException($"Unsupported payment scheme : {scheme}")
            };
        }
    }
}
