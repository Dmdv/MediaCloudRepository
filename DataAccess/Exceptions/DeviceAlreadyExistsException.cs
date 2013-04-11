using System;

namespace DataAccess.Exceptions
{
	public class DeviceAlreadyExistsException : DalException
	{
		public DeviceAlreadyExistsException(Guid traceId, string deviceName)
			: base(traceId, string.Format("Device {0} already exists", deviceName))
		{
			DeviceName = deviceName;
		}

		public string DeviceName { get; private set; }
	}
}