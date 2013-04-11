using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Extensions;
using DataAccess.Helpers;

namespace DataAccess.Repository
{
	public class QueryRepository : RepositoryBase<QueryHistory>, IQueryRepository
	{
		public QueryRepository(ITableStorageProvider<QueryHistory> tableStorage) 
			: base(tableStorage)
		{
		}

		public Task Add(QueryHistory query)
		{
			return AddInternal(query);
		}

		public Task Delete(QueryHistory query)
		{
			return DeleteInternal(query);
		}

		public void Delete(Guid deviceId, string fileName)
		{
			var queryHistories = Get(CreateSearchKey(deviceId)).Result.Where( query => query.FileName == fileName);
			DeleteMany(queryHistories);
		}

		public void Delete(Guid deviceId)
		{
			var queryHistories = Get(CreateSearchKey(deviceId)).Result;
			DeleteMany(queryHistories);
		}

		public QueryHistory GetOld(Guid deviceId)
		{
			return Get(CreateSearchKey(deviceId)).Result.OrderBy(x => x.QueryTime).FirstOrDefault();
		}

		public string IsExist(Guid deviceId)
		{
			var queryHistory = GetOld(deviceId);
			return queryHistory == null ? string.Empty : queryHistory.FileName;
		}

		private void DeleteMany(IEnumerable<QueryHistory> queryHistories)
		{
			Parallel.ForEach(queryHistories, query => DeleteInternal(query));

			//foreach (var queryHistory in queryHistories)
			//{
			//	DeleteInternal(queryHistory).Wait();
			//}
		}

		private static Tuple<string, string> CreateSearchKey(Guid deviceId)
		{
			Guard.CheckGuidNotEmpty(deviceId, "deviceId");
			return new Tuple<string, string>(deviceId.ToStringWithInvariantCulture(), string.Empty);
		}
	}
}