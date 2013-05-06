using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DataAccess.Repository
{
	public class BlobRepository : IBlobRepository
	{
		private readonly CloudBlobClient _blobClient;

		public BlobRepository(string connectionString)
		{
			var storageAccount = CloudStorageAccount.Parse(connectionString);
			_blobClient = storageAccount.CreateCloudBlobClient();
		}

		public Task SaveBlob(string containerName, string fileName, Stream stream)
		{
			CloudBlob cloudBlob = null;
			try
			{
				if (stream.CanSeek)
				{
					stream.Seek(0, SeekOrigin.Begin);
				}
				
				var blobContainer = CreateContainer(containerName);
				cloudBlob = CreateBlob(fileName, blobContainer);
				return Task.Factory.FromAsync(cloudBlob.BeginUploadFromStream, cloudBlob.EndUploadFromStream, stream, null);
			}
			catch (StorageException exception)
			{
				if (cloudBlob != null)
				{
					cloudBlob.DeleteIfExists();
				}
				throw new InvalidOperationException(
					string.Format(
						"Exception on read value from blob. Container: {0}, blob name: {1}, message {2}",
						containerName,
						fileName,
						exception.Message),
					exception);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="blobContainer"></param>
		/// <returns></returns>
		private static CloudBlob CreateBlob(string fileName, CloudBlobContainer blobContainer)
		{
			return blobContainer.GetBlobReference(fileName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="containerName"></param>
		/// <returns></returns>
		private CloudBlobContainer CreateContainer(string containerName)
		{
			var container = _blobClient.GetContainerReference(containerName);
			if (container.CreateIfNotExist())
			{
				var perm = new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob};
				container.SetPermissions(perm);
				container.Metadata.Add(new NameValueCollection());
			}

			return container;
		}
	}
}