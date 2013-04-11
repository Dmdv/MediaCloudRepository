using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class MediaAlreadyExistsFault : BaseFault
	{
		public MediaAlreadyExistsFault(string message)
			: base(message)
		{
		}

		public MediaAlreadyExistsFault(Guid deviceId, string fileName, Guid traceId)
			: base(string.Format("Media already exists. DeviceId:{0}, FileName:{1}", deviceId, fileName), traceId)
		{
			DeviceId = deviceId;
			FileName = fileName;
		}

		[DataMember]
		public Guid DeviceId { get; private set; }

		[DataMember]
		public string FileName { get; private set; }
	}
}