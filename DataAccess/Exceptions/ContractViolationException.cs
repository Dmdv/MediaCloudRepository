using System;
using System.Runtime.Serialization;

namespace DataAccess.Exceptions
{
	[Serializable]
	public sealed class ContractViolationException : Exception
	{
		public ContractViolationException()
		{
		}

		public ContractViolationException(string message)
			: base(message)
		{
		}

		public ContractViolationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		private ContractViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}