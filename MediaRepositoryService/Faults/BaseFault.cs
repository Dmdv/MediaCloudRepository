using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class BaseFault
	{
		public BaseFault(string message)
			: this(message, Guid.Empty)
		{
		}

		public BaseFault(string message, Guid traceId)
		{
			Message = message;
			TraceId = traceId;
		}

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public Guid TraceId { get; set; }
	}
}