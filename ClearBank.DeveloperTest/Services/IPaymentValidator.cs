using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentValidator
    {
        bool IsValid(Account account, MakePaymentRequest request);
    }
}
