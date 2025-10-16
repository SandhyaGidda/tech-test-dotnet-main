using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;
        private readonly PaymentValidatorFactory _validatorFactory;

        public PaymentService(IAccountDataStore accountDataStore, PaymentValidatorFactory validatorFactory)
        {
            _accountDataStore = accountDataStore;
            _validatorFactory = validatorFactory;
        }
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var result = new MakePaymentResult { Success = false };

            var account = _accountDataStore.GetAccount(request.DebtorAccountNumber);
            var validator = _validatorFactory.GetValidator(request.PaymentScheme);

            if (validator.IsValid(account, request))
            {
                account.Balance -= request.Amount;
                _accountDataStore.UpdateAccount(account);
                result.Success = true;
            }

            return result;
        }
    }
}
