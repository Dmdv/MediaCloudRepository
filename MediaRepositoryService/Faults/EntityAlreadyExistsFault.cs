using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class EntityAlreadyExistsFault : BaseFault
	{
		public object Entity { get; set; }

		public EntityAlreadyExistsFault(string message, object entity) :
			base(message)
		{
			Entity = entity;
		}
	}
}