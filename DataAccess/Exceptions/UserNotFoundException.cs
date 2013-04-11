using System;

namespace DataAccess.Exceptions
{
	public class UserNotFoundException : DalException
	{
		public UserNotFoundException(Guid traceId, string userName, string password, Guid userId)
			: base(traceId, string.Format("User Name: '{0}', Password: '{1}', UserId: '{2}' does not exist.", userName, password, userId))
		{
			UserId = userId;
		}

		public Guid UserId { get; private set; }
	}
}