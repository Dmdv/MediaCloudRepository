using System;
using System.IO;
using System.ServiceModel;
using MediaRepositoryWebRole.Faults;

namespace MediaRepositoryWebRole.Contracts
{
	[ServiceContract]
	public interface IMediaRepositoryService
	{
		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(UserNotFoundFault))]
		[FaultContract(typeof(EntityAlreadyExistsFault))]
		[FaultContract(typeof(InternalServerErrorFault))]
		IAsyncResult BeginCreateDevice(string userName, string password, Guid deviceGuid, string deviceName, AsyncCallback callback, object state);

		void EndCreateDevice(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(DeviceNotFoundFault))]
		[FaultContract(typeof(SerializeExceptionFault))]
		[FaultContract(typeof(EntityAlreadyExistsFault))]
		[FaultContract(typeof(InternalServerErrorFault))]
		IAsyncResult BeginUploadPreview(Stream previewInfoStream, AsyncCallback callback, object state);

		void EndUploadPreview(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(DeviceNotFoundFault))]
		[FaultContract(typeof(InternalServerErrorFault))]
		[FaultContract(typeof(SerializeExceptionFault))]
		IAsyncResult BeginUploadOriginal(Stream fileInfoStream, AsyncCallback callback, object state);

		void EndUploadOriginal(IAsyncResult result);

		[OperationContract]
		[FaultContract(typeof(InternalServerErrorFault))]
		string IsQueryExist(Guid deviceGuid);
	}
}