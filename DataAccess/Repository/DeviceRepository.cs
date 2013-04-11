using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	/// <summary>
	/// Репозиторий, управляющий устройствами.
	/// </summary>
	public class DeviceRepository : RepositoryBase<Device>, IDeviceRepository
	{
		public DeviceRepository(ITableStorageProvider<Device> tableStorage)
			: base(tableStorage)
		{
		}

		/// <summary>
		/// Создать устройство.
		/// </summary>
		public async Task Add(Device device)
		{
			await AddInternal(device);
		}

		/// <summary>
		/// Проверка существования устройства по Guid.
		/// </summary>
		public bool IsExist(Guid deviceId)
		{
			return Find(deviceId).Result.Count() != 0;
		}

		/// <summary>
		/// Проверка существования устройства по сущности.
		/// </summary>
		public bool IsExist(Device device)
		{
			return Get(DefaultSearchKey(device)).Result.Count() != 0;
		}

		/// <summary>
		/// Поиск устройства по ID пользователя.
		/// </summary>
		public async Task<IEnumerable<Device>> Find(Guid deviceId)
		{
			return await Get(DefaultSearchKey(new Device {DeviceId = deviceId}));
		}

		/// <summary>
		/// Поиск устройства по ID пользователя и имени устройства.
		/// </summary>
		public async Task<IEnumerable<Device>> Find(Guid deviceId, string deviceName)
		{
			return await Find(deviceId)
				             .ContinueWith(
					             x => x.Result.Where(device => device.DeviceName == deviceName),
					             TaskContinuationOptions.NotOnFaulted);
		}
	}
}