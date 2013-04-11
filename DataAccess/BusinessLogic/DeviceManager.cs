using System;
using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Exceptions;
using DataAccess.Helpers;
using DataAccess.Repository;

namespace DataAccess.BusinessLogic
{
	/// <summary>
	/// Управление созданием устройств.
	/// </summary>
	public class DeviceManager
	{
		private readonly IDeviceRepository _deviceRepository;
		private readonly IUserRepository _userRepository;

		public DeviceManager(
			ITableStorageProvider<Device> deviceContext,
			ITableStorageProvider<User> userContext)
		{
			_deviceRepository = new DeviceRepository(deviceContext);
			_userRepository = new UserRepository(userContext);
		}

		/// <summary>
		/// Создает устройство.
		/// </summary>
		/// <param name="deviceId">Id устройства.</param>
		/// <param name="deviceName">Имя устройства.</param>
		/// <param name="userName">Имя пользователя.</param>
		/// <param name="password">Пароль.</param>
		public Task CreateDevice(Guid deviceId, string deviceName, string userName, string password)
		{
			Guard.CheckGuidNotEmpty(deviceId, "deviceId");
			Guard.CheckContainsText(userName, "userName");
			Guard.CheckContainsText(password, "password");
			Guard.CheckContainsText(deviceName, "deviceName");

			var user = _userRepository.Find(userName, password, null);
			if (user == null)
			{
				throw new UserNotFoundException(Guid.NewGuid(), userName, password, Guid.Empty);
			}

			if (_deviceRepository.IsExist(deviceId))
			{
				throw new DeviceAlreadyExistsException(Guid.Empty, deviceName);
			}

			if (!user.UserId.HasValue)
			{
				throw new ArgumentException("user.UserId.HasValue");
			}

			return _deviceRepository.Add(new Device(deviceId, user.UserId.Value, deviceName));
		}
	}
}