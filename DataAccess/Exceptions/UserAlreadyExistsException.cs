using System;

namespace DataAccess.Exceptions
{
	public class UserAlreadyExistsException : DalException
	{
		public UserAlreadyExistsException(Guid traceId, Guid userId)
			: base(traceId, string.Format("User {0} already exists", userId))
		{
			UserId = userId;
		}

		public Guid UserId { get; private set; }
	}
}