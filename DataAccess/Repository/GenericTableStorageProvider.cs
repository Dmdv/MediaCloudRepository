using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace DataAccess.Repository
{
	public class GenericTableStorageProvider<TEntity> : 
		TableStorageProviderBase<TEntity, TEntity> where TEntity : TableServiceEntity
	{
		public GenericTableStorageProvider(string tableName, string connectionString)
			: base(tableName, connectionString)
		{
		}

		protected override TEntity ProcessGetResult(TEntity pEntity, TableServiceContext context)
		{
			return pEntity;
		}

		protected override TEntity GetPersistentEntity(TEntity entity)
		{
			return entity;
		}

		protected override string GetEtag(TEntity entity)
		{
			return "*";
		}
	}
}