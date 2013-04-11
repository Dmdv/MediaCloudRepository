using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace MediaRepositoryWebRole
{
	public class AzureLocalStorageTraceListener : XmlWriterTraceListener
	{
		public AzureLocalStorageTraceListener()
			: base(Path.Combine(GetLogDirectory().Path, "MediaRepositoryWebRole.svclog"))
		{
		}

		public static DirectoryConfiguration GetLogDirectory()
		{
			var directory = new DirectoryConfiguration
			{
				Container = "wad-tracefiles",
				DirectoryQuotaInMB = 10,
				Path = RoleEnvironment.GetLocalResource("MediaRepositoryWebRole.svclog").RootPath
			};
			return directory;
		}
	}
}
