using System;
using System.IO;
using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Exceptions;
using DataAccess.Helpers;
using DataAccess.Repository;
using DataAccess.Extensions;

namespace DataAccess.BusinessLogic
{
	/// <summary>
	/// Управление закачкой файлов.
	/// </summary>
	public class MediaManager
	{
		private readonly IQueryRepository _queryRepository;
		private readonly IBlobRepository _blobRepository;
		private readonly IDeviceRepository _deviceRepository;
		private readonly IMediaRepository _mediaRepository;

		public MediaManager(
			ITableStorageProvider<MediaFile> mediaStorageProvider, 
			ITableStorageProvider<QueryHistory> queryProvider, 
			ITableStorageProvider<Device> deviceStorageProvider, 
			IBlobRepository blobRepository)
		{
			_deviceRepository = new DeviceRepository(deviceStorageProvider);
			_mediaRepository = new MediaRepository(mediaStorageProvider);
			_queryRepository = new QueryRepository(queryProvider);
			_blobRepository = blobRepository;
		}

		/// <summary>
		/// Закачивает исходник.
		/// </summary>
		public Task UploadOriginal(Guid deviceId, string fileName, DateTime dateTime, Stream stream)
		{
			Guard.CheckGuidNotEmpty(deviceId, "deviceId");
			Guard.CheckContainsText(fileName, "fileName");

			if (!_deviceRepository.IsExist(deviceId))
			{
				throw new DeviceNotFoundException(Guid.NewGuid(), deviceId);
			}

			var mediaFile = new MediaFile(deviceId, fileName, dateTime, MediaSize.Source);

			if (stream == null)
			{
				return Task.Factory.StartNew(() => _queryRepository.Delete(deviceId, fileName));
			}

			return SaveStreamToBlob(mediaFile, stream)
				.ContinueWith(task => _queryRepository.Delete(deviceId, fileName), TaskContinuationOptions.NotOnFaulted);
		}

		/// <summary>
		/// Закачивает превью.
		/// </summary>
		public Task UploadPreview(Guid deviceId, string fileName, DateTime dateTime, Stream stream)
		{
			Guard.CheckGuidNotEmpty(deviceId, "deviceId");
			Guard.CheckContainsText(fileName, "fileName");

			if (!_deviceRepository.IsExist(deviceId))
			{
				throw new DeviceNotFoundException(Guid.NewGuid(), deviceId);
			}

			var mediaFile = new MediaFile(deviceId, fileName, dateTime, MediaSize.Preview);

			if (_mediaRepository.IsExist(mediaFile))
			{
				throw new MediaAlreadyExistsException(Guid.NewGuid(), fileName, dateTime, deviceId);
			}

			return SaveStreamToBlob(mediaFile, stream)
				.ContinueWith(task => _mediaRepository.Add(mediaFile), TaskContinuationOptions.NotOnFaulted);
		}

		/// <summary>
		/// Проверка существования запроса на закачку файла.
		/// </summary>
		public string IsQueryExist(Guid deviceId)
		{
			Guard.CheckGuidNotEmpty(deviceId, "deviceId");
			return _queryRepository.IsExist(deviceId);
		}

		/// <summary>
		/// Сохраняет поток в блоб.
		/// </summary>
		private Task SaveStreamToBlob(MediaFile mediaFile, Stream stream)
		{
			return _blobRepository.SaveBlob(mediaFile.GetBlobContainerName(), mediaFile.GetBlobFileName(), stream);
		}
	}
}