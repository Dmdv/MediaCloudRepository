using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class UserAlreadyExistsFault : BaseFault
	{
		public UserAlreadyExistsFault(Guid userId)
			: base(string.Format(CultureInfo.InvariantCulture, "UserAlreadyExistsFault userId: {0}", userId))
		{
			UserId = userId;
		}

		[DataMember]
		public Guid UserId { get; private set; }
	}
}