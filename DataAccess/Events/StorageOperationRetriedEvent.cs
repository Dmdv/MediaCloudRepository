using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace DataAccess.Events
{
	public class StorageOperationRetriedEvent : IStorageEvent
	{
		public StorageOperationRetriedEvent(
			Exception exception,
			string policy,
			int trial,
			TimeSpan interval,
			Guid trialSequence)
		{
			Exception = exception;
			Policy = policy;
			Trial = trial;
			Interval = interval;
			TrialSequence = trialSequence;
		}

		public Exception Exception { get; private set; }

		public string Policy { get; private set; }

		public int Trial { get; private set; }

		public TimeSpan Interval { get; private set; }

		public Guid TrialSequence { get; private set; }

		public TraceEventType Level
		{
			get { return TraceEventType.Information; }
		}

		public string Describe()
		{
			return
				string.Format(
					"Storage: Operation was retried on policy {0} ({1} trial): {2}",
					Policy,
					Trial,
					Exception != null ? Exception.Message : string.Empty);
		}

		public XElement DescribeMeta()
		{
			return new XElement(
				"Meta",
				new XElement("Component", "Cloud.DataAccess"),
				new XElement("Event", "StorageOperationRetriedEvent"));
		}
	}
}