using System;
using System.Runtime.Serialization;

namespace DataAccess.Exceptions
{
	[DataContract]
	public class StorageDataAccessException : Exception
	{
		public StorageDataAccessException(Exception innerException, Guid traceId, string message, string errorCode)
			: base(message, innerException)
		{
			TraceId = traceId;
			SourceComponent = "Frontend.DataAccess";
			ErrorCode = errorCode;
		}

		[DataMember]
		public string ErrorCode { get; set; }

		[DataMember]
		public Guid TraceId { get; set; }

		[DataMember]
		public string SourceComponent { get; set; }
	}
}