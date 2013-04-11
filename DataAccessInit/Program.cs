using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

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

			DeleteTableAsync(tableClient, ServiceFactory.ServiceSettings.DeviceTable);
			DeleteTableAsync(tableClient, ServiceFactory.ServiceSettings.UserTable);
			DeleteTableAsync(tableClient, ServiceFactory.ServiceSettings.MediaTable);
			DeleteTableAsync(tableClient, ServiceFactory.ServiceSettings.QueryHistory);

			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.DeviceTable);
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.UserTable);
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.MediaTable);
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.QueryHistory);
		}

		private static void DeleteTableAsync(CloudTableClient tableClient, string tableName)
		{
			if (tableClient.DoesTableExist(tableName))
			{
				Task.Factory.FromAsync(
					tableClient.BeginDeleteTable,
					tableClient.EndDeleteTable,
					tableName, null).Wait();
			}
		}
	}
}