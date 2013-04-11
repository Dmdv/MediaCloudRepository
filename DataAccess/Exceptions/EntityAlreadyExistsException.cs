using System;
using DataAccess.Events;

namespace DataAccess.Exceptions
{
	public class EntityAlreadyExistsException : Exception
	{
		public EntityAlreadyExistsException(string message, object entity, Guid traceId, string sourceComponent)
			: base(message)
		{
			Entity = entity;
			TraceId = traceId;
			SourceComponent = sourceComponent;
		}

		public object Entity { get; set; }

		public Guid TraceId { get; set; }

		public string SourceComponent { get; set; }
	}
}