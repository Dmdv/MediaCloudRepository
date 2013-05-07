using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DataAccess.Exceptions;
using DataAccess.Extensions;
using DataAccess.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
// using RetryPolicies = DataAccess.Helpers.RetryPolicies;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace DataAccess.Repository
{
	public abstract class TableStorageProviderBase<TEntity, TPersistentEntity> : ITableStorageProvider<TEntity>
		where TPersistentEntity : TableServiceEntity
	{
		protected TableStorageProviderBase(string tableName, string connectionString)
		{
			_tableName = tableName;
			var account = CloudStorageAccount.Parse(connectionString);
			_tableAccount = account.CreateCloudTableClient();
		}

		#region ITableStorageProvider<TEntity> Members

		public async Task ExecuteTransactionBatch(Batch<TEntity> batch)
		{
			Guard.CheckNotNull(batch, "batch");

			var context = _tableAccount.GetDataServiceContext();

			if (batch.Delete != null && batch.Delete.Count != 0)
			{
				DeleteEntities(context, batch.Delete);
			}

			if (batch.Insert != null && batch.Insert.Count != 0)
			{
				InsertEntities(context, batch.Insert);
			}

			if (batch.Update != null && batch.Update.Count != 0)
			{
				UpdateEntities(context, batch.Update);
			}

			if (batch.InsertOrReplace != null && batch.InsertOrReplace.Count != 0)
			{
				InsertOrReplaceEntities(context, batch.InsertOrReplace);
			}

			DataServiceResponse serviceResponse = null;
			try
			{
				serviceResponse = await Retry.Do(
					() =>
					Task<DataServiceResponse>.Factory.FromAsync(
						context.BeginSaveChanges,
						context.EndSaveChanges,
						SaveChangesOptions.Batch | SaveChangesOptions.ReplaceOnUpdate,
						null),
					new LinearRetry(),
					CancellationToken.None);
			}
			catch (Exception e)
			{
				Exception newException;
				if (ExceptionHandling.Handle(e, context, serviceResponse, out newException))
				{
					throw newException;
				}
				throw;
			}
		}

		public bool IsExist(Dictionary<string, string> hash)
		{
			return IsExistInternal(CreateFilter(hash));
		}

		public bool IsExist(IEnumerable<Tuple<string, string>> keys)
		{
			return IsExistInternal(CreateFilter(keys));
		}

		//public IAsyncEnumerable<TEntity> GetAsyncListOrderBy(string filter, string orderedField)
		//{
		//	Guard.CheckNotNull(filter, "filter");

		//	var context = _tableAccount.GetDataServiceContext();
		//	var query = CreateQueryForGetList(context,
		//		new Dictionary<string, string>
		//		{
		//			{"$filter", filter},
		//			{"$orderby", string.Format("\"{0}\"", orderedField)}
		//		});

		//	var asyncList = GetListInternal(query);

		//	return new AsyncEnumerableToAsyncTransformer<TEntity, TPersistentEntity>(asyncList, pEntity => ProcessGetResult(pEntity, context));
		//}

		//public IAsyncEnumerable<TEntity> GetAsyncListOrderBy(IEnumerable<Tuple<string, string>> keys, string fieldName)
		//{
		//	return GetAsyncListOrderBy(CreateFilter(keys), fieldName);
		//}

		public IAsyncEnumerable<TEntity> GetAsyncList(Dictionary<string, string> hash)
		{
			return GetAsyncList(CreateFilter(hash));
		}

		public IAsyncEnumerable<TEntity> GetAsyncList(IEnumerable<Tuple<string, string>> keys)
		{
			return GetAsyncList(CreateFilter(keys));
		}

		public IAsyncEnumerable<TEntity> GetAsyncList(string filter)
		{
			Guard.CheckNotNull(filter, "filter");

			var context = _tableAccount.GetDataServiceContext();
			var query = CreateQueryForGetList(context, new Dictionary<string, string> {{"$filter", filter}});
			var asyncList = GetListInternal(query);

			return new AsyncEnumerableToAsyncTransformer<TEntity, TPersistentEntity>(asyncList, pEntity => ProcessGetResult(pEntity, context));
		}

		#endregion

		protected abstract TEntity ProcessGetResult(TPersistentEntity pEntity, TableServiceContext context);

		protected abstract TPersistentEntity GetPersistentEntity(TEntity entity);

		protected abstract string GetEtag(TEntity entity);

		internal static string CreateFilter(Dictionary<string, string> hash)
		{
			Guard.CheckNotNull(hash, "hash");
			return string.Join(" and ", hash.Select(x => string.Format("({0} eq '{1}')", x.Key, HttpUtility.UrlEncode(x.Value))));
		}

		private static string CreateFilter(IEnumerable<Tuple<string, string>> keys)
		{
			var tuples = keys.ToArray();
			ValidateKeys(tuples);
			return string.Join(
				"or",
				tuples.Select(keyPair =>
				{
					if (string.IsNullOrWhiteSpace(keyPair.Item2))
					{
						return string.Format("(PartitionKey eq '{0}')", HttpUtility.UrlEncode(keyPair.Item1));
					}

					if (string.IsNullOrWhiteSpace(keyPair.Item1))
					{
						return string.Format("(RowKey eq '{0}')", HttpUtility.UrlEncode(keyPair.Item2));
					}

					return string.Format(
						"(PartitionKey eq '{0}') and (RowKey eq '{1}')", HttpUtility.UrlEncode(keyPair.Item1), HttpUtility.UrlEncode(keyPair.Item2));
				}));
		}

		private CloudTableQuery<TPersistentEntity> CreateQueryForGetList(TableServiceContext context, Dictionary<string, string> filters)
		{
			var query = context.CreateQuery<TPersistentEntity>(_tableName);
			query = filters.Aggregate(query, (current, keyValuePair) => current.AddQueryOption(keyValuePair.Key, keyValuePair.Value));
			return query.AsTableServiceQuery();
		}

		private bool IsExistInternal(string filter)
		{
			var context = _tableAccount.GetDataServiceContext();
			var query = CreateQueryForGetList(context, new Dictionary<string, string> {{"$filter", filter}});
			return query.FirstOrDefault() != null;
		}

		private void InsertOrReplaceEntities(DataServiceContext context, IEnumerable<TEntity> updated)
		{
			var persistentEntities = updated.Select(GetPersistentEntity).ToArray();

			ValidatePersistentEntities(persistentEntities);

			persistentEntities.ForEach(
				persistentEntity =>
					{
						context.AttachTo(_tableName, persistentEntity);
						context.UpdateObject(persistentEntity);
					});
		}

		private void InsertEntities(DataServiceContext context, IEnumerable<TEntity> inserted)
		{
			var persistentEntities = inserted.Select(GetPersistentEntity).ToArray();
			ValidatePersistentEntities(persistentEntities);
			persistentEntities.ForEach(persistentEntity => context.AddObject(_tableName, persistentEntity));
		}

		private void UpdateEntities(DataServiceContext context, IEnumerable<TEntity> updated)
		{
			foreach (var fatEntity in updated)
			{
				var persistentEntity = GetPersistentEntity(fatEntity);
				context.AttachTo(_tableName, persistentEntity, GetEtag(fatEntity));
				context.UpdateObject(persistentEntity);
			}
		}

		private void DeleteEntities(DataServiceContext context, IEnumerable<TEntity> deleted)
		{
			foreach (var fatEntity in deleted)
			{
				var persistentEntity = GetPersistentEntity(fatEntity);
				context.AttachTo(_tableName, persistentEntity, GetEtag(fatEntity));
				context.DeleteObject(persistentEntity);
			}
		}

		private static void ValidatePersistentEntities(TPersistentEntity[] persistentEntities)
		{
			var invalidEntity = persistentEntities.FirstOrDefault(left => persistentEntities.Count(right => left.RowKey == right.RowKey) > 1);
			if (invalidEntity != null)
			{
				throw new BatchDuplicateEntityException(invalidEntity.PartitionKey, invalidEntity.RowKey);
			}
		}

		private static void ValidateKeys(IEnumerable<Tuple<string, string>> keys)
		{
			foreach (var key in keys)
			{
				if (string.IsNullOrWhiteSpace(key.Item1) && string.IsNullOrWhiteSpace(key.Item2))
				{
					throw new ArgumentException("Both PartitionKey and Row key are null or empty");
				}

				var item1 = key.Item1 ?? string.Empty;
				var item2 = key.Item2 ?? string.Empty;

				if (item1.IndexOf('\'') != -1)
				{
					throw new ArgumentException(string.Format("PartitionKey contains symbol \"'\". Key: {0}", item1));
				}
				
				if (item2.IndexOf('\'') != -1)
				{
					throw new ArgumentException(string.Format("RowKey contains symbol \"'\". Key: {0}", item2));
				}
			}
		}

		private IAsyncEnumerable<IEnumerable<TPersistentEntity>> GetListInternal(
			CloudTableQuery<TPersistentEntity> tableQuery)
		{
			ResultSegment<TPersistentEntity> resultSegment = null;

			return new GenericAsyncEnumerable<IEnumerable<TPersistentEntity>>(
				() => new GenericAsyncEnumerator<IEnumerable<TPersistentEntity>>(
					      moveNext: cancellationToken =>
						                {
							                var tcs = new TaskCompletionSource<bool>();
							                Task<ResultSegment<TPersistentEntity>> task;

							                if (resultSegment == null)
							                {
								                task = Retry.Do(
									                () => Task<ResultSegment<TPersistentEntity>>.Factory.FromAsync(
										                tableQuery.BeginExecuteSegmented,
										                tableQuery.EndExecuteSegmented,
										                (ResultContinuation)null,
										                null),
									                new LinearRetry(),
									                CancellationToken.None);
							                }
							                else
							                {
								                if (!resultSegment.HasMoreResults)
								                {
									                return TaskHelpers.FromResult(false);
								                }

								                // ReSharper disable AccessToModifiedClosure
								                task = Retry.Do(
									                () => Task<ResultSegment<TPersistentEntity>>.Factory.FromAsync(
										                resultSegment.BeginGetNext, resultSegment.EndGetNext, null),
													new LinearRetry(),
									                CancellationToken.None);
								                // ReSharper restore AccessToModifiedClosure
							                }

							                task.ContinueWith(
								                t =>
									                {
										                if (t.IsFaulted)
										                {
											                // ReSharper disable AssignNullToNotNullAttribute
											                tcs.SetException(HandleException(t.Exception));
											                // ReSharper restore AssignNullToNotNullAttribute
										                }
										                else if (t.IsCanceled)
										                {
											                tcs.SetCanceled();
										                }
										                else
										                {
											                resultSegment = t.Result;
											                tcs.SetResult(true);
										                }
									                },
								                TaskContinuationOptions.ExecuteSynchronously);

							                return tcs.Task;
						                },
					      current: () =>
						               {
							               if (resultSegment == null)
							               {
								               throw new InvalidOperationException("resultSegment == null");
							               }

							               return resultSegment.Results;
						               },
					      dispose: () =>
						               {
						               }));
		}

		private Exception HandleException(Exception exception)
		{
			var trialSequence = Guid.NewGuid();
			const string Message = "Operation GetListInternal failed";
			return ExceptionHandling.MapToDataAccessException(exception, trialSequence, Message);
		}

		private readonly CloudTableClient _tableAccount;
		private readonly string _tableName;
	}
}