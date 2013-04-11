using System.ServiceModel;
using MediaRepositoryWebRole.Faults;

namespace MediaRepositoryWebRole.Extensions
{
	internal static class FaultExtensions
	{
		public static FaultException<TFault> ToFaultException<TFault>(this TFault fault) where TFault : BaseFault
		{
			return new FaultException<TFault>(fault, fault.Message);
		}
	}
}