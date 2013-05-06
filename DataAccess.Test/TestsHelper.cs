using DataAccess.Extensions;
using MediaRepositoryWebRole;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.DeviceTable);
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.UserTable);
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.MediaTable);
			tableClient.CreateTableIfNotExists(ServiceFactory.ServiceSettings.QueryHistory);
		}
	}
}