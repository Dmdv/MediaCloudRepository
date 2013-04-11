using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class InternalServerErrorFault : BaseFault
	{
		public InternalServerErrorFault(InternalServerErrorFaultErrorCode errorCode)
			: base(string.Format("Internal error with code {0}", (int)errorCode))
		{
			ErrorCode = (int)errorCode;
		}

		public InternalServerErrorFault(InternalServerErrorFaultErrorCode errorCode, Guid traceId)
			: base(string.Format("Internal error with code {0}", (int)errorCode), traceId)
		{
			ErrorCode = (int)errorCode;
		}

		[DataMember]
		public int ErrorCode { get; set; }
	}
}