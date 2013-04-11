using System;
using System.ServiceModel;
using MediaRepositoryWebRole.Data;

namespace MediaRepositoryWebRole.Contracts
{
	[ServiceContract]
	public interface IUserManager
	{
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCreateUser(User user, AsyncCallback callback, object state);

		void EndCreateUser(IAsyncResult result);
	}
}