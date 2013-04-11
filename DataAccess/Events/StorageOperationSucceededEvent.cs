using System;
using System.Diagnostics;
using System.Xml.Linq;
using DataAccess.Repository;

namespace DataAccess.Events
{
	/// <summary>
	/// Raised whenever a storage operation has succeeded.
	/// Useful for collecting usage statistics.
	/// </summary>
	public class StorageOperationSucceededEvent : IStorageEvent
	{
		public StorageOperationSucceededEvent(StorageOperationType operationType, TimeSpan duration)
		{
			OperationType = operationType;
			Duration = duration;
		}

		public StorageOperationType OperationType { get; private set; }

		public TimeSpan Duration { get; private set; }

		public TraceEventType Level
		{
			get { return TraceEventType.Information; }
		}

		public string Describe()
		{
			return string.Format(
				"Storage: {0} operation succeeded in {1:0.00}s",
				OperationType,
				Duration.TotalSeconds);
		}

		public XElement DescribeMeta()
		{
			return new XElement(
				"Meta",
				new XElement("Component", "Cloud.DataAccess"),
				new XElement("Event", "StorageOperationSucceededEvent"));
		}
	}
}