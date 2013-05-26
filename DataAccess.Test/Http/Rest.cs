using System.IO;
using MediaRepositoryWebRole;
using Newtonsoft.Json;

namespace DataAccess.Test.Http
{
	public static class Rest
	{
		public static string PostStream(string command, Stream stream)
		{
			var client = new RestClient(
				endpoint: ServiceFactory.ServiceSettings.Uri + command,
				method: HttpVerb.POST)
				{
					ContentType = Application.OcteatStream
				};

			return client.MakeStreamRequest(stream);
		}

		public static string Post(string command, object @object)
		{
			var client = new RestClient(
				endpoint: ServiceFactory.ServiceSettings.Uri + command,
				method: HttpVerb.POST,
				postData: JsonConvert.SerializeObject(@object));

			return client.MakeRequest();
		}

		public static string Get(string command)
		{
			var client = new RestClient(
				endpoint: ServiceFactory.ServiceSettings.Uri + command,
				method: HttpVerb.GET);

			return client.MakeRequest();
		}
	}
}