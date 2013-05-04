using System;
using DataAccess.Extensions;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace DataAccess.Entities
{
	/// <summary>
	/// Устройство.
	/// </summary>
	public sealed class Device : TableServiceEntity
	{
		private Guid? _userId;
		private Guid? _deviceId;

		public Device()
		{
		}

		public Device(Guid deviceId, Guid userId, string deviceName)
		{
			DeviceId = deviceId;
			UserId = userId;
			DeviceName = deviceName;
			CreateKeys();
		}

		public Guid? UserId
		{
			get { return _userId; }
			set
			{
				_userId = value;
				PartitionKey = CreatePartitionKey();
			}
		}

		public Guid? DeviceId
		{
			get { return _deviceId; }
			set
			{
				_deviceId = value;
				RowKey = CreateRowKey();
			}
		}

		public string DeviceName { get; set; }

		private void CreateKeys()
		{
			PartitionKey = CreatePartitionKey();
			RowKey = CreateRowKey();
		}

		private string CreatePartitionKey()
		{
			return UserId.ToStringWithInvariantCulture();
		}

		private string CreateRowKey()
		{
			return DeviceId.ToStringWithInvariantCulture();
		}
	}
}