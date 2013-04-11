using System;
using System.Runtime.Serialization;

namespace DataAccess.Exceptions
{
	[Serializable]
	public class InvariantViolationException : Exception
	{
		public InvariantViolationException()
		{
		}

		public InvariantViolationException(string message)
			: base(message)
		{
		}

		public InvariantViolationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvariantViolationException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}