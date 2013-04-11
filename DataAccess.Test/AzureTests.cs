using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DataAccess.BusinessLogic;
using DataAccess.Entities;
using DataAccess.Helpers;
using DataAccess.Repository;
using MediaRepositoryWebRole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.StorageClient;
using DataAccess.Extensions;
using User = DataAccess.Entities.User;

namespace DataAccess.Test
{
	[TestClass]
	[DeploymentItem(@"..\..\..\DataAccess.Test\trace.jpg")]
	[DeploymentItem(@"..\..\..\DataAccess.Test\Image.jpg")]
	public class AzureTests
	{
		private const string Image = "Image.jpg";
		private const string FileName = "trace.jpg";
		private User _user;
		private ITableStorageProvider<User> _userContext;
		private ITableStorageProvider<Device> _deviceContext;
		private CloudBlobClient _blobClient;

		[TestInitialize]
		public void TestInitialize()
		{
			_user = new User(Guid.NewGuid().ToStringWithInvariantCulture(), Guid.NewGuid().ToStringWithInvariantCulture())
			{
				UserId = Guid .NewGuid()
			};

			_userContext = ServiceFactory.CreateUserContext();
			_deviceContext = ServiceFactory.CreateDeviceContext();
			_blobClient = TestsHelper.InitializeAzure();
		}

		[TestMethod]
		public void TestCreateUser()
		{
			var repository = new UserRepository(_userContext);
			repository.Add(_user).Wait();
			Assert.IsTrue(repository.IsExist(_user.Name, _user.Password, _user.UserId));
			Assert.IsFalse(repository.IsExist(_user.Name, "111", _user.UserId));
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void TestCreateExistingUser()
		{
			var repository = new UserRepository(_userContext);
			repository.Add(_user).Wait();
			repository.Add(_user).Wait();
		}

		[TestMethod]
		public void TestCreateAndFindUser()
		{
			UserRepository repo;
			var user = CreateAndAddUsuer(out repo);
			var find = repo.Find(user.Name, user.Password, user.UserId);
			Assert.IsTrue(find.Password == user.Password);
			Assert.IsTrue(find.Name == user.Name);
			Assert.IsTrue(find.UserId == user.UserId);
			find = repo.Find(user.Name, "112", user.UserId);
			Assert.IsNull(find);
		}

		private User CreateAndAddUsuer(out UserRepository repo)
		{
			var user = new User(Guid.NewGuid().ToStringWithInvariantCulture(), Guid.NewGuid().ToString()) {UserId = _user.UserId};
			repo = new UserRepository(_userContext);
			repo.Add(user).Wait();
			return user;
		}

		[TestMethod]
		public void TestCreateAndDeleteHistory()
		{
			var query = new QueryHistory(Guid.NewGuid(), "12345", DateTime.UtcNow);

			var repository = new QueryRepository(ServiceFactory.CreateQueryHistoryContext());
			repository.Add(query).Wait();
			Assert.IsTrue(repository.IsExist(query.DeviceId) != string.Empty);
			repository.Delete(query).Wait();
			Assert.IsTrue(repository.IsExist(query.DeviceId) == string.Empty);
		}

		[TestMethod]
		public void TestGetOldestHistory()
		{
			var deviceId = Guid.NewGuid();
			var query = new QueryHistory(deviceId, "new", DateTime.UtcNow);
			var queryOld = new QueryHistory(deviceId, "old", DateTime.UtcNow.AddDays(-1));

			var repository = new QueryRepository(ServiceFactory.CreateQueryHistoryContext());
			repository.Add(query).Wait();
			repository.Add(queryOld).Wait();

			Assert.IsTrue(repository.IsExist(query.DeviceId) == "old");
		}

		[TestMethod]
		public void TestDateTimeSerializer()
		{
			var dateTime = new DateTime(2008, 3, 9, 16, 5, 7, 123);
			var serialize = DateTimeSerializer.Serialize(dateTime);
			Assert.IsTrue(serialize == "20080309160507");
			var deserialize = DateTimeSerializer.Deserialize(serialize);
			Assert.IsTrue(deserialize.Year == dateTime.Year);
			Assert.IsTrue(deserialize.Month == dateTime.Month);
			Assert.IsTrue(deserialize.Date == dateTime.Date);
			Assert.IsTrue(deserialize.Hour == dateTime.Hour);
			Assert.IsTrue(deserialize.Minute == dateTime.Minute);
			Assert.IsTrue(deserialize.Second == dateTime.Second);

			var media = new MediaFile(Guid.NewGuid(), "sample.jpg", dateTime, MediaSize.Preview);
			Assert.IsTrue(media.RowKey == "20080309160507");
		}

		[TestMethod]
		public void TestCreateDevice()
		{
			UserRepository repo;
			var user = CreateAndAddUsuer(out repo);
			if (!user.UserId.HasValue)
			{
				throw new ArgumentException("guid");
			}

			var repository = new DeviceRepository(_deviceContext);

			var device1 = new Device(Guid.NewGuid(), user.UserId.Value, Guid.NewGuid().ToString());
			Debug.Assert(device1.DeviceId != null, "device1.DeviceId != null");
			Assert.IsFalse(repository.IsExist((Guid) device1.DeviceId));

			repository.Add(device1).Wait();
			Assert.IsTrue(repository.IsExist((Guid) device1.DeviceId));
			Assert.IsNotNull(repository.Find((Guid) device1.DeviceId, device1.DeviceName).Result.FirstOrDefault());
		}

		[TestMethod]
		public void TestUploadToBlob()
		{
			UserRepository repo;
			var user = CreateAndAddUsuer(out repo);
			if (!user.UserId.HasValue)
			{
				throw new ArgumentException("guid");
			}

			var deviceId = Guid.NewGuid();
			var repository = new DeviceRepository(_deviceContext);
			var device = new Device(deviceId, user.UserId.Value, Guid.NewGuid().ToString());
			repository.Add(device).Wait();

			var manager = new MediaManager(
				ServiceFactory.CreateMediaFileContext(),
				ServiceFactory.CreateQueryHistoryContext(),
				ServiceFactory.CreateDeviceContext(),
				ServiceFactory.CreateBlobRepository());

			using (var fileStream = File.Open(Image, FileMode.Open))
			{
				manager.UploadPreview(deviceId, Image, DateTime.UtcNow, fileStream).Wait();
			}

			using (var fileStream = File.Open(FileName, FileMode.Open))
			{
				manager.UploadOriginal(deviceId, FileName, DateTime.UtcNow, fileStream).Wait();
			}

			var blobContainer = _blobClient.GetContainerReference(deviceId.ToStringWithInvariantCulture());
			Assert.AreEqual(blobContainer.Name, deviceId.ToStringWithInvariantCulture());
		}

		[TestMethod]
		public void TestSearchFilter()
		{
			var query = new Dictionary<string, string> { { "Param1", "Value1" }, { "Param2", "Value2" }, { "Param3", "Value3" } };
			var filter = TableStorageProviderBase<Device, Device>.CreateFilter(query);
			Assert.AreEqual(filter, "(Param1 eq 'Value1') and (Param2 eq 'Value2') and (Param3 eq 'Value3')");
		}
	}
}