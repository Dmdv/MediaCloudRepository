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

		[OperationContract(Name = "CreateUser2", AsyncPattern = true)]
		[WebInvoke(
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.WrappedRequest,
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json,
			UriTemplate = "/creates/user")]
		IAsyncResult BeginCreateUser2(User user, User user2, AsyncCallback callback, object state);

		User EndCreateUser2(IAsyncResult result);

		[OperationContract(Name = "IsUserExists", AsyncPattern = false)]
		[WebInvoke(
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json,
			UriTemplate = "/isuserexists")]
		bool IsUserExists(User user);
	}
}