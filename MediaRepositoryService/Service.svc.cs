using System;
using System.IO;
using System.Threading.Tasks;
using DataAccess.BusinessLogic;
using DataAccess.Exceptions;
using MediaRepositoryWebRole.Contracts;
using User = DataAccess.Entities.User;
using MediaRepositoryWebRole.Extensions;

namespace MediaRepositoryWebRole
{
	public class MediaRepositoryService : ServiceBase, IMediaRepositoryService, IUserManager
	{
		private readonly UserManager _userManager;
		private readonly DeviceManager _deviceManager;
		private readonly MediaManager _mediaManager;

		public MediaRepositoryService()
		{
			_userManager = new UserManager(
				ServiceFactory.CreateUserContext());

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

		public string IsQueryExist(Guid deviceGuid)
		{
			return _mediaManager.IsQueryExist(deviceGuid);
		}

		public IAsyncResult BeginCreateUser(Data.User user, AsyncCallback callback, object state)
		{
			var task = _userManager.CreateUser(new User(user.Name, user.Password) {UserId = user.UserId});
			return task.AsAsyncResult(callback, state);
		}

		public void EndCreateUser(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}
	}
}