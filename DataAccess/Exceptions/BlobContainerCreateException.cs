using System;

namespace DataAccess.Exceptions
{
	public class BlobContainerCreateException : DalException
	{
		public BlobContainerCreateException(Guid traceId, string blobContainerName)
			: base(traceId, string.Format("Failed to create blob container '{0}' ", blobContainerName))
		{
			ContainerName = blobContainerName;
		}

		public string ContainerName { get; private set; }
	}
}