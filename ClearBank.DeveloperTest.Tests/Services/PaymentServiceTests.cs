using Xunit;
using Moq;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Data;
using System.Configuration;

namespace ClearBank.DeveloperTest.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IAccountDataStore> _accountDataStoreMock = new();
        private readonly PaymentValidatorFactory _validatorFactory = new();

        // -----------------------
        //  Bacs Scheme Tests
        // -----------------------

        [Fact]
        public void MakePayment_Bacs_AllowedAccount_ShouldReturnSuccessAndUpdateBalance()
        {
            // Arrange
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 100m
            };
            _accountDataStoreMock.Setup(x => x.GetAccount("123")).Returns(account);

            var service = new PaymentService(_accountDataStoreMock.Object, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 10m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(90m, account.Balance);
            _accountDataStoreMock.Verify(x => x.UpdateAccount(account), Times.Once);
        }

        [Fact]
        public void MakePayment_Bacs_NotAllowedScheme_ShouldFail()
        {
            // Arrange
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 200m
            };
            _accountDataStoreMock.Setup(x => x.GetAccount("123")).Returns(account);

            var service = new PaymentService(_accountDataStoreMock.Object, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 50m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            _accountDataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        // -----------------------
        //  Faster Payments Tests
        // -----------------------

        [Fact]
        public void MakePayment_FasterPayments_InsufficientFunds_ShouldFail()
        {
            // Arrange
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 5m
            };
            _accountDataStoreMock.Setup(x => x.GetAccount("123")).Returns(account);

            var service = new PaymentService(_accountDataStoreMock.Object, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 10m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            _accountDataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void MakePayment_FasterPayments_SufficientFunds_ShouldSucceedAndUpdateBalance()
        {
            // Arrange
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 100m
            };
            _accountDataStoreMock.Setup(x => x.GetAccount("123")).Returns(account);

            var service = new PaymentService(_accountDataStoreMock.Object, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 25m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(75m, account.Balance);
            _accountDataStoreMock.Verify(x => x.UpdateAccount(account), Times.Once);
        }

        // -----------------------
        //  CHAPS Tests
        // -----------------------

        [Fact]
        public void MakePayment_Chaps_NonLiveAccount_ShouldFail()
        {
            // Arrange
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };
            _accountDataStoreMock.Setup(x => x.GetAccount("123")).Returns(account);

            var service = new PaymentService(_accountDataStoreMock.Object, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            _accountDataStoreMock.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void MakePayment_Chaps_LiveAccount_ShouldSucceed()
        {
            // Arrange
            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };
            _accountDataStoreMock.Setup(x => x.GetAccount("123")).Returns(account);

            var service = new PaymentService(_accountDataStoreMock.Object, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 20m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.True(result.Success);
            _accountDataStoreMock.Verify(x => x.UpdateAccount(account), Times.Once);
        }

        // --------------------------------------------
        //  Integration Test – AccountDataStoreFactory
        // --------------------------------------------

        [Fact]
        public void PaymentService_ShouldWorkWithFactoryCreatedDataStore()
        {
            // Arrange
            ConfigurationManager.AppSettings["DataStoreType"] = "Backup";

            // Factory should select BackupAccountDataStore
            var dataStore = AccountDataStoreFactory.CreateFromConfig();
            Assert.IsType<BackupAccountDataStore>(dataStore);

            var service = new PaymentService(dataStore, _validatorFactory);

            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "1",
                Amount = 50m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // Act
            var result = service.MakePayment(request);

            // Assert
            Assert.NotNull(result);
        }
    }
}
