using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataAccess.Extensions
{
	public static class CloudTableClientExtension
	{
		public static void CreateTableIfNotExists(this CloudTableClient tableClient, string tableName)
		{
			var tableReference = tableClient.GetTableReference(tableName);
			tableReference.CreateIfNotExists();
		}

		public static void DeleteTableAsync(this CloudTableClient tableClient, string tableName)
		{
			var tableReference = tableClient.GetTableReference(tableName);
			Task.Factory.FromAsync<bool>(tableReference.BeginDeleteIfExists, tableReference.EndDeleteIfExists, null).Wait();
		}
	}
}