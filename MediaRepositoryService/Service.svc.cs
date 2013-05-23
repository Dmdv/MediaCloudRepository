using System;
using System.IO;
using System.Threading.Tasks;
using DataAccess.BusinessLogic;
using DataAccess.Exceptions;
using MediaRepositoryWebRole.Contracts;
using MediaRepositoryWebRole.Extensions;
using User = DataAccess.Entities.User;

namespace MediaRepositoryWebRole
{
	public class MediaRepositoryService : ServiceBase,
		IMediaRepositoryService, 
		IUserManager
	{
		private readonly UserManager _userManager;
		private readonly DeviceManager _deviceManager;
		private readonly MediaManager _mediaManager;

		public MediaRepositoryService()
		{
			var userContext = ServiceFactory.CreateUserContext();
			_userManager = new UserManager(
				userContext);

			_deviceManager = new DeviceManager(
				ServiceFactory.CreateDeviceContext(), 
				ServiceFactory.CreateUserContext());

			_mediaManager = new MediaManager(
				ServiceFactory.CreateMediaFileContext(),
				ServiceFactory.CreateQueryHistoryContext(),
				ServiceFactory.CreateDeviceContext(),
				ServiceFactory.CreateBlobRepository());
		}

		public IAsyncResult BeginCreateDevice(string userName, string password, Guid deviceGuid, string deviceName, AsyncCallback callback, object state)
		{
			var task = _deviceManager.CreateDevice(deviceGuid, deviceName, userName, password);
			return task.AsAsyncResult(callback, state);
		}

		public void EndCreateDevice(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}

		public IAsyncResult BeginUploadPreview(Stream previewInfoStream, AsyncCallback callback, object state)
		{
			var filStreamInfo = previewInfoStream.Deserialize();
			var task = _mediaManager.UploadPreview(filStreamInfo.DeviceId, filStreamInfo.FileName, filStreamInfo.DateTime, filStreamInfo.Stream);
			return task.AsAsyncResult(callback, state);
		}

		public void EndUploadPreview(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}

		public IAsyncResult BeginUploadOriginal(Stream fileInfoStream, AsyncCallback callback, object state)
		{
			var filStreamInfo = fileInfoStream.Deserialize();
			var task = _mediaManager.UploadOriginal(filStreamInfo.DeviceId, filStreamInfo.FileName, filStreamInfo.DateTime, filStreamInfo.Stream);
			return task.AsAsyncResult(callback, state);
		}

		public void EndUploadOriginal(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}

		public IAsyncResult BeginCreateUser(Data.User user, AsyncCallback callback, object state)
		{
			var task = _userManager.CreateUser(new User(user.Name, user.Password) { UserId = user.UserId });
			return task.AsAsyncResult(callback, state);
		}

		public void EndCreateUser(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}

		// {"user":{"Name":"111","Password":"111","UserId":"00000000-0000-0000-0000-000000000000"},"user2":{"Name":null,"Password":null,"UserId":"00000000-0000-0000-0000-000000000000"}}

		public IAsyncResult BeginCreateUser2(Data.User user, Data.User user2, AsyncCallback callback, object state)
		{
			var task = Task.Run(() => new Data.User());
			return task.AsAsyncResult(callback, state);
		}

		public Data.User EndCreateUser2(IAsyncResult result)
		{
			var task = (Task<Data.User>) result;
			return ProcessWithExceptionShield(() => task.Result);
		}

		public bool IsUserExists(Data.User user)
		{
			return _userManager.IsUserExists(
				new User
				{
					Name = user.Name, 
					Password = user.Password, 
					UserId = user.UserId
				});
		}
	}
}