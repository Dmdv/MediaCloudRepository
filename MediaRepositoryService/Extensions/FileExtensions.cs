using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MediaRepositoryWebRole.Data;

namespace MediaRepositoryWebRole.Extensions
{
	internal static class FileExtensions
	{
		private const int BufferSize = 1024 * 4;

		public static string SaveToFile(this FileStreamInfo fileStreamInfo)
		{
			return fileStreamInfo.Stream.SaveStreamToFile(fileStreamInfo.FileName);
		}

		public static FileStreamInfo Deserialize(this Stream stream)
		{
			var buffer = new byte[BufferSize];

			using (var writeStream = new MemoryStream())
			{
				var read = stream.Read(buffer, 0, BufferSize);

				while (read > 0)
				{
					writeStream.Write(buffer, 0, read);
					read = stream.Read(buffer, 0, BufferSize);
				}

				if (writeStream.CanSeek)
				{
					writeStream.Seek(0, SeekOrigin.Begin);
				}
				
				var formatter = new BinaryFormatter();
				return (FileStreamInfo)formatter.Deserialize(writeStream);
			}
		}

		private static string TempFileName(this string name)
		{
			return Path.Combine(Path.GetTempPath(), name);
		}

		private static string SaveStreamToFile(this Stream stream, string fileName)
		{
			var buffer = new byte[BufferSize];
			var tempFileName = fileName.TempFileName();

			using (var fs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				var read = stream.Read(buffer, 0, BufferSize);

				while (read > 0)
				{
					fs.Write(buffer, 0, read);
					read = stream.Read(buffer, 0, BufferSize);
				}
			}

			return tempFileName;
		}
	}
}