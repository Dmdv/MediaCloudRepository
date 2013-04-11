using System;
using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Data
{
	[DataContract]
	public class User
	{
		[DataMember(IsRequired = true)]
		public Guid UserId { get; set; }

		[DataMember(IsRequired = true)]
		public string Name { get; set; }

		[DataMember(IsRequired = true)]
		public string Password { get; set; }
	}
}