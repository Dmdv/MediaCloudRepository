namespace DataAccess.Repository
{
	public enum StorageOperationType
	{
		BlobPut,
		BlobGet,
		BlobGetIfModified,
		BlobUpsertOrSkip,
		BlobDelete,
		TableQuery,
		TableInsert,
		TableUpdate,
		TableDelete,
		TableUpsert,
		QueueGet,
		QueuePut,
		QueueDelete,
		QueueAbandon,
		QueuePersist,
		QueueWrap,
		QueueUnwrap
	}
}