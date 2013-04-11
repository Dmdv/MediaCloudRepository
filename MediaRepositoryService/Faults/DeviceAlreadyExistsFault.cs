using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class DeviceAlreadyExistsFault : BaseFault
	{
		public DeviceAlreadyExistsFault(string message)
			: base(message)
		{
		}

		public DeviceAlreadyExistsFault(Guid userId, string deviceId, Guid traceId)
			: base(string.Format("Device already exists. UserId:{0}, DeviceId:{1}", userId, deviceId), traceId)
		{
			UserId = userId;
			DeviceId = deviceId;
		}

		[DataMember]
		public Guid UserId { get; private set; }

		[DataMember]
		public string DeviceId { get; private set; }
	}
}