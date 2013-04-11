using MediaRepositoryWebRole;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

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
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.DeviceTable);
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.UserTable);
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.MediaTable);
			tableClient.CreateTableIfNotExist(ServiceFactory.ServiceSettings.QueryHistory);
		}
	}
}