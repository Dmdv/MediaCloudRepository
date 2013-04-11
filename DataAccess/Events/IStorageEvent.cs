using System.Diagnostics;
using System.Xml.Linq;

namespace DataAccess.Events
{
	public interface IStorageEvent
	{
		TraceEventType Level { get; }

		string Describe();

		XElement DescribeMeta();
	}
}