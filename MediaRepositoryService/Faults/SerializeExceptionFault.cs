using System.Runtime.Serialization;

namespace MediaRepositoryWebRole.Faults
{
	[DataContract]
	public class SerializeExceptionFault : BaseFault
	{
		public SerializeExceptionFault() : 
			base("Stream serialization error")
		{
		}
	}
}