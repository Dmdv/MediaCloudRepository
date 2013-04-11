using System;
using DataAccess.Extensions;

namespace DataAccess.Exceptions
{
	public class BatchDuplicateEntityException : Exception
	{
		public BatchDuplicateEntityException(string partitionKey, string rowKey)
			: base("Entity with partition key '{0}' and row key '{1}' is duplicated in the batch."
				       .FormatString(partitionKey, rowKey))
		{
		}
	}
}