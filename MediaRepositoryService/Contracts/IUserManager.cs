using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using MediaRepositoryWebRole.Data;

namespace MediaRepositoryWebRole.Contracts
{
	[ServiceContract]
	public interface IUserManager
	{
		[OperationContract(Name = "CreateUser", AsyncPattern = true)]
		[WebInvoke(
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json,
			UriTemplate = "/create/user")]
		IAsyncResult BeginCreateUser(User user, AsyncCallback callback, object state);

		void EndCreateUser(IAsyncResult result);
	}
}