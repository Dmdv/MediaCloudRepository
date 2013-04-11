using System;

namespace DataAccess.Exceptions
{
	public class MediaAlreadyExistsException : DalException
	{
		public MediaAlreadyExistsException(Guid traceId, string filename, DateTime dateTime, Guid deviceId)
			: base(traceId, string.Format("Media {0} already exists", filename))
		{
			FileName = filename;
			DateTime = dateTime;
			DeviceId = deviceId;
		}

		public string FileName { get; private set; }
		public DateTime DateTime { get; set; }
		public Guid DeviceId { get; set; }
	}
}