using System.Threading.Tasks;
using DataAccess.Repository;

namespace DataAccess.Extensions
{
	public static class TableStorageProviderExtensions
	{
		public static Task InsertEntity<TEntity>(this ITableStorageProvider<TEntity> storageProvider, TEntity entity)
		{
			return storageProvider.ExecuteTransactionBatch(new Batch<TEntity>(inserted: new[] {entity}));
		}

		public static Task InsertEntityAndUpdateOne<TEntity>(
			this ITableStorageProvider<TEntity> storageProvider,
			TEntity insert,
			TEntity update)
		{
			return storageProvider
					.ExecuteTransactionBatch(
						new Batch<TEntity>(
							inserted: new[] {insert},
							updated: new[] {update}));
		}

		public static Task InsertEntityAndDeleteOne<TEntity>(this ITableStorageProvider<TEntity> storageProvider,
		                                                     TEntity delete, TEntity insert)
		{
			return storageProvider
				.ExecuteTransactionBatch(
					new Batch<TEntity>(
						deleted: new[] {delete},
						inserted: new[] {insert}));
		}
	}
}