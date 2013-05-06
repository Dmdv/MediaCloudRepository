using DataAccess.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataAccessInit
{
	public static class Program
	{
		private static void Main()
		{
			InitializeAzure();
		}

		private static void InitializeAzure()
		{
			var account = CloudStorageAccount.Parse(ServiceFactory.ServiceSettings.ConnectionString);
			InitializeTables(account);
		}

		private static void InitializeTables(CloudStorageAccount account)
		{
			var tableClient = account.CreateCloudTableClient();

			tableClient.DeleteTableAsync(ServiceFactory.ServiceSettings.DeviceTable);
			tableClient.DeleteTableAsync(ServiceFactory.ServiceSettings.UserTable);
			tableClient.DeleteTableAsync(ServiceFactory.ServiceSettings.MediaTable);
			tableClient.DeleteTableAsync(ServiceFactory.ServiceSettings.QueryHistory);

			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.DeviceTable);
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.UserTable);
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.MediaTable);
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.QueryHistory);
		}
	}
}