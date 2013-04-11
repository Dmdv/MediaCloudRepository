using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Extensions;
using DataAccess.Helpers;
using Microsoft.WindowsAzure.StorageClient;

namespace DataAccess.Repository
{
	public abstract class RepositoryBase<TEntity>
	{
		protected RepositoryBase(
			ITableStorageProvider<TEntity> tableStorage)
		{
			_tableStorage = tableStorage;
		}

		protected bool IsExist(Tuple<string, string> searchKeys)
		{
			return _tableStorage.IsExist(searchKeys.YieldArray());
		}

		protected bool IsExist(Dictionary<string, string> hash)
		{
			return _tableStorage.IsExist(hash);
		}

		protected async Task<IEnumerable<TEntity>> Get(Dictionary<string, string> hash)
		{
			return await GetInternalEntities(_tableStorage.GetAsyncList(hash));
		}

		protected Task<IEnumerable<TEntity>> Get(Tuple<string, string> searchKey)
		{
			return Get(searchKey.YieldArray());
		}

		protected async Task<IEnumerable<TEntity>> Get(IEnumerable<Tuple<string, string>> searchKeys)
		{
			return await GetInternalEntities(_tableStorage.GetAsyncList(searchKeys));
		}

		//protected Task<IEnumerable<TEntity>> GetOrderBy(Tuple<string, string> searchKey, string fieldName)
		//{
		//	return GetOrderBy(searchKey.YieldArray(), fieldName);
		//}

		//protected async Task<IEnumerable<TEntity>> GetOrderBy(IEnumerable<Tuple<string, string>> searchKeys, string fieldName)
		//{
		//	return await GetInternalEntities(_tableStorage.GetAsyncListOrderBy(searchKeys, fieldName));
		//}

		protected async Task AddInternal(TEntity entity)
		{
			await _tableStorage.InsertEntity(entity);
		}

		protected async Task DeleteInternal(TEntity entity)
		{
			await _tableStorage.ExecuteTransactionBatch(new Batch<TEntity>(deleted: entity.YieldArray()));
		}

		private static async Task<IEnumerable<TEntity>> GetInternalEntities(IAsyncEnumerable<TEntity> retrievedList)
		{
			var result = new List<TEntity>();

			using (var enumerator = retrievedList.GetEnumerator())
			{
				while (await enumerator.MoveNext(CancellationToken.None))
				{
					result.Add(enumerator.Current);
				}
			}

			return result;
		}

		protected Tuple<string, string> DefaultSearchKey(TableServiceEntity entyty)
		{
			Guard.CheckTrue(entyty.PartitionKey != null || entyty.RowKey != null,
				"either RowKey or PartitionKey must not be null" );
			return new Tuple<string, string>(entyty.PartitionKey, entyty.RowKey);
		}

		private readonly ITableStorageProvider<TEntity> _tableStorage;
	}
}