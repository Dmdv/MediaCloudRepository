using System.Configuration;
using DataAccess.Entities;
using DataAccess.Repository;

namespace MediaRepositoryWebRole
{
	/// <summary>
	/// Временный класс для хранения и создания глобальных статических объектов.
	/// Можно заменить на Unity или Windsor.
	/// </summary>
	public static class ServiceFactory
	{
		private static readonly ServiceSettings _serviceSettings = new ServiceSettings();

		static ServiceFactory()
		{
			_serviceSettings.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
			_serviceSettings.DeviceTable = ConfigurationManager.AppSettings["DeviceTable"];
			_serviceSettings.MediaTable = ConfigurationManager.AppSettings["MediaTable"];
			_serviceSettings.QueryHistory = ConfigurationManager.AppSettings["QueryHistory"];
			_serviceSettings.UserTable = ConfigurationManager.AppSettings["UserTable"];
		}

		public static ITableStorageProvider<Device> CreateDeviceContext()
		{
			return new GenericTableStorageProvider<Device>(_serviceSettings.DeviceTable, _serviceSettings.ConnectionString);
		}

		public static ITableStorageProvider<User> CreateUserContext()
		{
			return new GenericTableStorageProvider<User>(_serviceSettings.UserTable, _serviceSettings.ConnectionString);
		}

		public static ITableStorageProvider<MediaFile> CreateMediaFileContext()
		{
			return new GenericTableStorageProvider<MediaFile>(_serviceSettings.MediaTable, _serviceSettings.ConnectionString);
		}

		public static ITableStorageProvider<QueryHistory> CreateQueryHistoryContext()
		{
			return new GenericTableStorageProvider<QueryHistory>(_serviceSettings.QueryHistory, _serviceSettings.ConnectionString);
		}

		public static IBlobRepository CreateBlobRepository()
		{
			return new BlobRepository(_serviceSettings.ConnectionString);
		}

		public static ServiceSettings ServiceSettings
		{
			get { return _serviceSettings; }
		}
	}
}