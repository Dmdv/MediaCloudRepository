using System;
using DataAccess.Extensions;
using DataAccess.Helpers;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace DataAccess.Entities
{
	/// <summary>
	/// История запросов.
	/// </summary>
	public sealed class QueryHistory : TableServiceEntity
	{
		private Guid _deviceId;
		private DateTime _queryTime;

		public QueryHistory()
		{
		}

		public QueryHistory(Guid deviceId, string fileName, DateTime queryTime)
		{
			DeviceId = deviceId;
			QueryId = new Guid();
			FileName = fileName;
			QueryTime = queryTime;

			CreateKeys();
		}

		public Guid QueryId { get; set; }

		public Guid DeviceId
		{
			get { return _deviceId; }
			set
			{
				_deviceId = value;
				PartitionKey = CreatePartitionKey();
			}
		}

		public string FileName { get; set; }

		public DateTime QueryTime
		{
			get { return _queryTime; }
			set
			{
				_queryTime = value;
				RowKey = CreateRowKey();
			}
		}

		private string CreatePartitionKey()
		{
			return DeviceId.ToStringWithInvariantCulture();
		}

		private string CreateRowKey()
		{
			return DateTimeSerializer.Serialize(QueryTime);
		}

		private void CreateKeys()
		{
			PartitionKey = CreatePartitionKey();
			RowKey = CreateRowKey();
		}
	}
}