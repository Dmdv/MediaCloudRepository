using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class DeviceNotFoundFault : BaseFault
	{
		public DeviceNotFoundFault(string message)
			: base(message)
		{
		}

		public DeviceNotFoundFault(string deviceId, Guid traceId)
			: base(string.Format("Device not found. DeviceId:{0}", deviceId), traceId)
		{
			DeviceId = deviceId;
		}

		[DataMember]
		public string DeviceId { get; private set; }
	}
}