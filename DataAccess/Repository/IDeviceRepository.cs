using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	/// <summary>
	/// Репозиторий устройств.
	/// </summary>
	public interface IDeviceRepository
	{
		/// <summary>
		/// Создать устройство.
		/// </summary>
		Task Add(Device device);

		/// <summary>
		/// Проверка существования устройства по Guid.
		/// </summary>
		bool IsExist(Guid deviceId);

		/// <summary>
		/// Проверка существования устройства по сущности.
		/// </summary>
		bool IsExist(Device device);

		/// <summary>
		/// Поиск устройства по ID пользователя и имени устройства.
		/// </summary>
		Task<IEnumerable<Device>> Find(Guid userId, string deviceName);

		/// <summary>
		/// Поиск устройства по ID пользователя.
		/// </summary>
		Task<IEnumerable<Device>> Find(Guid deviceId);
	}
}