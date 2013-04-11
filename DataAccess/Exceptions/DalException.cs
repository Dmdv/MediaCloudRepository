using System;

namespace DataAccess.Exceptions
{
	[Serializable]
	public class DalException : Exception
	{
		public DalException(Guid traceId, string message, Exception innerException)
			: base(message, innerException)
		{
			TraceId = traceId;
		}

		public DalException(Guid traceId, string message)
			: base(message)
		{
			TraceId = traceId;
		}

		public Guid TraceId { get; private set; }
	}
}