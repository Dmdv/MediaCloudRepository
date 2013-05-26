using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using MediaRepositoryWebRole.Data;
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
		[WebInvoke(
			Method = "POST",
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			UriTemplate = "/create/device")]
		IAsyncResult BeginCreateDevice(CreateDeviceParam param, AsyncCallback callback, object state);

		void EndCreateDevice(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(DeviceNotFoundFault))]
		[FaultContract(typeof(SerializeExceptionFault))]
		[FaultContract(typeof(EntityAlreadyExistsFault))]
		[FaultContract(typeof(InternalServerErrorFault))]
		[WebInvoke(
			Method = "POST", 
			RequestFormat = WebMessageFormat.Json, 
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			UriTemplate = "/uploadpreview/{filename}/{deviceid}")]
		IAsyncResult BeginUploadPreview(string filename, string deviceId, Stream stream, AsyncCallback callback, object state);

		void EndUploadPreview(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(DeviceNotFoundFault))]
		[FaultContract(typeof(InternalServerErrorFault))]
		[FaultContract(typeof(SerializeExceptionFault))]
		[WebInvoke(
			Method = "POST", 
			RequestFormat = WebMessageFormat.Json, 
			ResponseFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			UriTemplate = "/uploadoriginal/{filename}/{deviceid}")]
		IAsyncResult BeginUploadOriginal(string filename, string deviceId, Stream stream, AsyncCallback callback, object state);

		void EndUploadOriginal(IAsyncResult result);
	}
}