using ClearBank.DeveloperTest.Data;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public static class AccountDataStoreFactory
    {
        public static IAccountDataStore CreateFromConfig()
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            return dataStoreType?.Equals("Backup", System.StringComparison.OrdinalIgnoreCase) == true
                     ? new BackupAccountDataStore()
                     : new AccountDataStore();
        }
    }
}
