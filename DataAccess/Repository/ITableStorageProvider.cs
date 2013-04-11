using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Helpers;

namespace DataAccess.Repository
{
	public interface ITableStorageProvider<TEntity>
	{
		Task ExecuteTransactionBatch(Batch<TEntity> batch);
		IAsyncEnumerable<TEntity> GetAsyncList(IEnumerable<Tuple<string, string>> keys);
		IAsyncEnumerable<TEntity> GetAsyncList(string filter);
		IAsyncEnumerable<TEntity> GetAsyncList(Dictionary<string, string> hash);
		bool IsExist(Dictionary<string, string> hash);
		bool IsExist(IEnumerable<Tuple<string, string>> keys);
		//IAsyncEnumerable<TEntity> GetAsyncListOrderBy(string filter, string orderedField);
		//IAsyncEnumerable<TEntity> GetAsyncListOrderBy(IEnumerable<Tuple<string, string>> keys, string fieldName);
	}
}