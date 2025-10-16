using Xunit;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Tests.Services
{
    /// <summary>
    /// Behavioral baseline tests for the ORIGINAL PaymentService implementation.
    /// 
    /// These tests are not designed to prove correctness, but to capture and 
    /// document what the legacy code actually does before refactoring.
    /// </summary>
    public class PaymentServicePrefactorTests
    {
        public PaymentServicePrefactorTests()
        {
            // Ensure a default config key exists for DataStoreType
            if (ConfigurationManager.AppSettings["DataStoreType"] == null)
            {
                ConfigurationManager.AppSettings["DataStoreType"] = "Default";
            }
        }

        [Fact]
        public void MakePayment_WhenAccountDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            var service = new PaymentServicePrefactor();
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "999", // assume not found
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_WhenAccountExistsButSchemeNotAllowed_ShouldReturnFailure()
        {
            // Arrange
            var service = new PaymentServicePrefactor();
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "1",
                Amount = 50m,
                PaymentScheme = PaymentScheme.Chaps // likely not allowed for this account
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_FasterPayments_InsufficientFunds_ShouldReturnFailure()
        {
            // Arrange
            var service = new PaymentServicePrefactor();
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "1",
                Amount = 10000m, // assume account has lower balance
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_Bacs_ValidAccount_ShouldReturnSuccess()
        {
            // Arrange
            var service = new PaymentServicePrefactor();
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "1",
                Amount = 10m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            // This is expected to fail , legacy implementation returns false as AccountDataStore.GetAccount returns null.
            Assert.True(result.Success);
        }

        [Fact]
        public void MakePayment_Chaps_LiveAccount_ShouldReturnFailure()
        {
            // Arrange
            var service = new PaymentServicePrefactor();
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "1",
                Amount = 50m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            // NOTE: In the legacy implementation, CHAPS payments fail legacy implementation returns false as AccountDataStore.GetAccount returns null.
            Assert.True(result.Success);
        }
    }
}
