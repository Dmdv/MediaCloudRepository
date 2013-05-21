using Microsoft.WindowsAzure.Storage;

namespace DataAccess.Repository
{
	internal static class StorageAccountFactory
	{
		public static CloudStorageAccount CreateCloudStorageAccount(string connectionString)
		{
			var localStorage = CloudStorageAccount.DevelopmentStorageAccount.ToString();
			return string.CompareOrdinal(localStorage, connectionString) == 0
				       ? CloudStorageAccount.DevelopmentStorageAccount
				       : CloudStorageAccount.Parse(connectionString);
		}
	}
}
