using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class UserNotFoundFault : BaseFault
	{
		public UserNotFoundFault(Guid userId)
			: base(string.Format(CultureInfo.InvariantCulture, "UserNotFoundFault userId: {0}", userId))
		{
			UserId = userId;
		}

		[DataMember]
		public Guid UserId { get; private set; }
	}
}