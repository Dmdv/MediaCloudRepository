using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Data
{
	[DataContract]
	public class CreateDeviceParam
	{
		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public Guid DeviceGuid { get; set; }
		
		[DataMember]
		public string DeviceName { get; set; }
	}
}