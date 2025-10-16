# ClearBank Developer Test – Refactor and Testing Notes

## Overview
The original `PaymentService` class was a single, tightly coupled component that handled configuration, data access, validation, and business logic within one method.  
This made it difficult to test, extend, or understand at a glance.  

The goal of the refactor was to improve:
- **Adherence to SOLID principles**
- **Testability**
- **Readability and maintainability**

## Key Changes
### Architectural Improvements
- **Dependency Inversion:** Introduced `IAccountDataStore` as a constructor dependency, removing hardcoded data store creation.
- **Single Responsibility:** Extracted validation logic for each payment scheme into individual `IPaymentValidator` implementations.
- **Open/Closed Principle:** Added a `PaymentValidatorFactory` to create the correct validator based on payment scheme, allowing new schemes to be added without modifying existing code.
- **Separation of Concerns:** `PaymentService` now orchestrates the process rather than performing validation directly.

### Design Patterns Used
During the refactor, I applied several design patterns to separate concerns and improve testability.
- **Strategy Pattern** in `IPaymentValidator` and its concrete implementations
- **Factory Pattern** in `PaymentValidatorFactory` and `AccountDataStoreFactory`
- **Dependency Injection (IoC)** in `PaymentService` constructor parameters (`IAccountDataStore, PaymentValidatorFactory`)
- **Interface Segregation & Abstraction Pattern** in `IPaymentService, IAccountDataStore, IPaymentValidator`

### Testing Enhancements
- Added comprehensive **unit tests** in the `ClearBank.DeveloperTest.Tests` project using **xUnit** and **Moq**.
- Tests follow the **Arrange–Act–Assert** pattern for readability.
- Covered scenarios for:
  - Successful Bacs, FasterPayments, and Chaps transactions.
  - Failure due to insufficient funds, inactive accounts, or invalid schemes.
  - Verification that account updates occur only when payments succeed.

### Approach
1. **Read & understand** the original implementation.
2. **Wrote baseline tests** for the legacy service to capture expected behavior.
3. **Refactored incrementally**, ensuring behavioral parity.
4. **Re-tested** using mocks and isolated validators.

### Pre refactor service
- Renamed old PaymentService.cs to `PaymentServicePrefactor.cs`
- Old logic tests are in `PaymentServicePrefactorTests.cs`

### Design Trade-offs
Due to time constraints, I focused on the core service logic and unit test coverage.  
Given more time, I would:
- Add **structured validation feedback** (reason codes, messages).
- Introduce **async/await** for data access methods.
- Implement **dependency injection** setup (example via .NET DI container).
- Add **integration tests** using an in-memory data store.
- Include **logging** for better observability.

## Outcome
The solution now builds cleanly, all tests pass, and the design supports future extension and easy test coverage.  
The refactored `PaymentService` is simpler to read, mock, and reason about, achieving the intended balance between clarity, correctness, and flexibility.
