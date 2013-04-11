using System;
using System.Diagnostics;
using System.Xml.Linq;
using DataAccess.Repository;

namespace DataAccess.Events
{
	public class StorageOperationExceptionEvent : IStorageEvent
	{
		public StorageOperationExceptionEvent(
			Exception exception,
			StorageOperationType operationType,
			string message,
			Guid trialSequence)
		{
			Exception = exception;
			Message = message;
			TrialSequence = trialSequence;
			OperationType = operationType;
		}

		public Exception Exception { get; private set; }

		public string Message { get; private set; }

		public StorageOperationType OperationType { get; private set; }

		public Guid TrialSequence { get; private set; }

		public TraceEventType Level
		{
			get { return TraceEventType.Error; }
		}

		public string Describe()
		{
			return string.Format(
				"Storage: Operation exception {0} OperationType: {1} ({2} trial): Exception: {3}",
				Message,
				OperationType,
				TrialSequence,
				Exception != null ? Exception.Message : string.Empty);
		}

		public XElement DescribeMeta()
		{
			return new XElement(
				"Meta",
				new XElement("Component", "Cloud.DataAccess"),
				new XElement("Event", "StorageOperationExceptionEvent"));
		}
	}
}