using System;

namespace DataAccess.Exceptions
{
	public class DeviceNotFoundException : DalException
	{
		public DeviceNotFoundException(Guid traceId, Guid deviceId)
			: base(traceId, string.Format("Device with id '{0}' hasn't been found.", deviceId))
		{
			DeviceId = deviceId;
		}

		public Guid DeviceId { get; set; }
	}
}