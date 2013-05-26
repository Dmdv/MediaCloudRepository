using System;
using System.IO;
using System.Threading.Tasks;
using DataAccess.BusinessLogic;
using DataAccess.Exceptions;
using MediaRepositoryWebRole.Contracts;
using MediaRepositoryWebRole.Data;
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

		public IAsyncResult BeginCreateDevice(CreateDeviceParam param, AsyncCallback callback, object state)
		{
			var task = _deviceManager.CreateDevice(param.DeviceGuid, param.DeviceName, param.UserName, param.Password);
			return task.AsAsyncResult(callback, state);
		}

		public void EndCreateDevice(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}

		public IAsyncResult BeginUploadPreview(
			string filename,
			string deviceId,
			Stream stream, AsyncCallback callback, object state)
		{
			var task = _mediaManager.UploadPreview(Guid.Parse(deviceId), filename, DateTime.UtcNow, stream);
			return task.AsAsyncResult(callback, state);
		}

		public void EndUploadPreview(IAsyncResult result)
		{
			ProcessWithExceptionShield(() => ((Task)result).Wait());
		}

		public IAsyncResult BeginUploadOriginal(
			string filename,
			string deviceId,
			Stream stream, AsyncCallback callback, object state)
		{
			var task = _mediaManager.UploadOriginal(Guid.Parse(deviceId), filename, DateTime.UtcNow, stream);
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