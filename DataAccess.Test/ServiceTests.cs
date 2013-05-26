using System;
using System.IO;
using System.Linq;
using System.Net;
using DataAccess.Entities;
using DataAccess.Repository;
using DataAccess.Test.Http;
using MediaRepositoryWebRole;
using MediaRepositoryWebRole.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using User = MediaRepositoryWebRole.Data.User;

namespace DataAccess.Test
{
	[TestClass]
	[DeploymentItem(@"..\..\..\DataAccess.Test\trace.jpg")]
	[DeploymentItem(@"..\..\..\DataAccess.Test\Image.jpg")]
	public class ServiceTests
	{
		private const string Image = "Image.jpg";
		private const string Trace = "trace.jpg";

		// private static IMediaRepositoryService _mediaRepository;
		// private static IUserManager _userManager;

		private static ITableStorageProvider<Entities.User> _userContext;
		private static ITableStorageProvider<Device> _deviceContext;
		private static CloudBlobClient _blobClient;

		private User _user;
		private string _deviceName;
		private Guid _deviceId;

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			// _userManager = new ServiceInterfaceProxy<IUserManager>(string.Empty).GetInterface();
			// _mediaRepository = new ServiceInterfaceProxy<IMediaRepositoryService>(string.Empty).GetInterface();
			_userContext = ServiceFactory.CreateUserContext();
			_deviceContext = ServiceFactory.CreateDeviceContext();
			_blobClient = TestsHelper.InitializeAzure();
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_user = new User
			{
				UserId = Guid.NewGuid(),
				Name = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString()
			};

			_deviceName = Guid.NewGuid().ToString();
			_deviceId = Guid.NewGuid();
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateUserRemotely()
		{
			Assert.AreEqual(CreateUser(), string.Empty);

			var repository = new UserRepository(_userContext);
			Assert.IsTrue(repository.IsExist(_user.Name, _user.Password, _user.UserId));
			Assert.IsFalse(repository.IsExist(_user.Name, "111", _user.UserId));
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateUserTwiceRemotely()
		{
			Assert.AreEqual(CreateUser(), string.Empty);
			try
			{
				Assert.AreEqual(CreateUser(), string.Empty);
			}
			catch (WebException ex)
			{
				AssertBadRequest(ex);
			}
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateDeviceAndCheck()
		{
			Assert.AreEqual(CreateUser(), string.Empty);
			Assert.AreEqual(CreateDevice(), string.Empty);

			var repository = new DeviceRepository(_deviceContext);

			Assert.IsTrue(repository.IsExist(_deviceId));
			Assert.IsNotNull(repository.Find(_deviceId, _deviceName).Result.FirstOrDefault());
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateDeviceTwiceRemotely()
		{
			Assert.AreEqual(CreateUser(), string.Empty);
			Assert.AreEqual(CreateDevice(), string.Empty);

			try
			{
				Assert.AreEqual(CreateDevice(), string.Empty);
			}
			catch (WebException ex)
			{
				AssertBadRequest(ex);
			}
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateDeviceUserNotFound()
		{
			try
			{
				Assert.AreEqual(CreateDevice(), string.Empty);
			}
			catch (WebException ex)
			{
				AssertBadRequest(ex);
			}
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateDeviceAndUploadPreview()
		{
			Assert.AreEqual(CreateUser(), string.Empty);
			Assert.AreEqual(CreateDevice(), string.Empty);

			UploadRest(Image, RestCommand.UploadPreview);

			var blobContainer = _blobClient.GetContainerReference(_deviceId.ToString());
			Assert.AreEqual(blobContainer.Name, _deviceId.ToString());
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestCreateDeviceAndUploadOriginal()
		{
			Assert.AreEqual(CreateUser(), string.Empty);
			Assert.AreEqual(CreateDevice(), string.Empty);

			UploadRest(Trace, RestCommand.UploadOriginal);

			var blobContainer = _blobClient.GetContainerReference(_deviceId.ToString());
			Assert.AreEqual(blobContainer.Name, _deviceId.ToString());
		}

		[TestCategory("ServiceTests")]
		[TestMethod]
		public void TestUploadPreviewDeviceNotFound()
		{
			try
			{
				UploadRest(Image, RestCommand.UploadPreview);
			}
			catch (WebException ex)
			{
				AssertBadRequest(ex);
			}
		}

		//private void UploadPreview(string fileName)
		//{
		//	using (var memoryStream = new MemoryStream(File.ReadAllBytes(fileName)))
		//	{
		//		using (var writeStream = new MemoryStream())
		//		{
		//			var preview = new FileStreamInfo
		//			{
		//				DateTime = DateTime.Now,
		//				FileName = fileName,
		//				DeviceId = _deviceId,
		//				Stream = memoryStream
		//			};

		//			var formatter = new BinaryFormatter();
		//			formatter.Serialize(writeStream, preview);

		//			if (writeStream.CanSeek)
		//			{
		//				writeStream.Seek(0, SeekOrigin.Begin);
		//			}

		//			var asyncResult = _mediaRepository.BeginUploadPreview(writeStream, null, null);
		//			Task.Factory.FromAsync(asyncResult, _mediaRepository.EndUploadPreview).Wait();
		//		}
		//	}
		//}

		private void UploadRest(string fileName, string command)
		{
			using (var memoryStream = new MemoryStream(File.ReadAllBytes(fileName)))
			{
				command = command
					.Replace("{filename}", fileName)
					.Replace("{deviceid}", _deviceId.ToString());

				Rest.PostStream(command, memoryStream);
			}
		}

		private string CreateUser()
		{
			return Rest.Post(RestCommand.CreateUser, _user);
		}

		private string CreateDevice()
		{
			var param = new CreateDeviceParam
			{
				DeviceName = _deviceName,
				DeviceGuid = _deviceId,
				Password = _user.Password,
				UserName = _user.Name
			};

			return Rest.Post(RestCommand.CreateDevice, param);

			//var asyncResult = _mediaRepository.BeginCreateDevice(param, null, null);
			//Task.Factory.FromAsync(asyncResult, _mediaRepository.EndCreateDevice).Wait();
		}

		private static void AssertBadRequest(WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				var response = ex.Response as HttpWebResponse;
				if (response != null)
				{
					Assert.AreEqual((int) response.StatusCode, 400);
				}
				else
				{
					Assert.Fail("Waiting for error status code");
				}
			}
			else
			{
				Assert.Fail("Waiting for error status code");
			}
		}
	}
}