using System;
using DataAccess.Helpers;
using DataAccess.Extensions;
using Microsoft.WindowsAzure.StorageClient;

namespace DataAccess.Entities
{
	public sealed class MediaFile : TableServiceEntity
	{
		private readonly MediaSize _postfix;
		private DateTime _dateTime;
		private Guid _deviceId;

		public MediaFile()
		{
		}

		public MediaFile(Guid deviceId, string fileName, DateTime dateTime, MediaSize postfix)
		{
			_postfix = postfix;

			MediaId = Guid.NewGuid();
			DeviceId = deviceId;
			DateTime = dateTime;
			FileName = fileName;

			CreateKeys();
		}

		public Guid DeviceId
		{
			get { return _deviceId; }
			set
			{
				_deviceId = value;
				PartitionKey = CreatePartitionKey();
			}
		}

		public Guid MediaId { get; set; }

		public string FileName { get; set; }

		public DateTime DateTime
		{
			get { return _dateTime; }
			set
			{
				_dateTime = value;
				RowKey = CreateRowKey();
			}
		}

		public int MediaType { get; set; }

		public string GetBlobContainerName()
		{
			return DeviceId.ToStringWithInvariantCulture();
		}

		public string GetBlobFileName()
		{
			return string.Format("{0}{1}", DateTimeSerializer.Serialize(DateTime), _postfix.ToString().ToLowerInvariant());
		}

		private string CreatePartitionKey()
		{
			return DeviceId.ToStringWithInvariantCulture();
		}

		private string CreateRowKey()
		{
			return DateTimeSerializer.Serialize(DateTime);
		}

		private void CreateKeys()
		{
			PartitionKey = CreatePartitionKey();
			RowKey = CreateRowKey();
		}
	}
}