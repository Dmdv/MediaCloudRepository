using MediaRepositoryWebRole;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataAccess.Test
{
	internal static class TestsHelper
	{
		internal static CloudBlobClient InitializeAzure()
		{
			var account = CloudStorageAccount.Parse(ServiceFactory.ServiceSettings.ConnectionString);
			InitializeTables(account);
			return account.CreateCloudBlobClient();
		}

		private static void InitializeTables(CloudStorageAccount account)
		{
			var tableClient = account.CreateCloudTableClient();

			CreateTableIfNotExists(tableClient, ServiceFactory.ServiceSettings.DeviceTable);
			CreateTableIfNotExists(tableClient, ServiceFactory.ServiceSettings.UserTable);
			CreateTableIfNotExists(tableClient, ServiceFactory.ServiceSettings.MediaTable);
			CreateTableIfNotExists(tableClient, ServiceFactory.ServiceSettings.QueryHistory);
		}

		private static void CreateTableIfNotExists(CloudTableClient tableClient, string tableName)
		{
			var tableReference = tableClient.GetTableReference(tableName);
			tableReference.CreateIfNotExists();
		}
	}
}