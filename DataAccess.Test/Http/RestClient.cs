using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using MediaRepositoryWebRole.Extensions;

namespace DataAccess.Test.Http
{
	public class RestClient
	{
		public RestClient(string endpoint, HttpVerb method)
			: this(endpoint, method, string.Empty)
		{
		}

		public RestClient(string endpoint, HttpVerb method, string postData)
		{
			EndPoint = endpoint;
			Method = method;
			ContentType = Application.Json;
			PostData = postData;
		}

		public string EndPoint { get; set; }

		public HttpVerb Method { get; set; }

		public string ContentType { get; set; }

		public string PostData { get; set; }

		// TODO: Избавиться от параметра.
		public string MakeRequest(string parameters = "")
		{
			var request = WebRequest.Create(EndPoint + parameters);
			WriteBody(request);
			return GetResponse(request);
		}

		public string MakeStreamRequest(Stream stream, string parameters = "")
		{
			var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
			WriteStreamToBody(request, stream);
			return GetResponse(request);
		}

		private void WriteBody(WebRequest request)
		{
			SetupRequest(request);

			if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
			{
				var encoding = new UTF8Encoding();
				var bytes = encoding.GetBytes(PostData);
				request.ContentLength = bytes.Length;

				using (var writeStream = request.GetRequestStream())
				{
					writeStream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		private void WriteStreamToBody(HttpWebRequest request, Stream stream)
		{
			SetupRequest(request);

			request.KeepAlive = false;
			request.Timeout = (int) TimeSpan.FromMinutes(10).TotalMilliseconds;
			request.ReadWriteTimeout = (int) TimeSpan.FromMinutes(10).TotalMilliseconds;
			request.SendChunked = true;
			request.AllowWriteStreamBuffering = false;

			try
			{
				using (var writeStream = request.GetRequestStream())
				{
					stream.SaveToStream(writeStream);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		private void SetupRequest(WebRequest request)
		{
			request.Method = Method.ToString();
			request.ContentLength = 0;
			request.ContentType = ContentType;
		}

		private static string GetResponse(WebRequest request)
		{
			using (var response = (HttpWebResponse) request.GetResponse())
			{
				var responseValue = string.Empty;

				if (response.StatusCode != HttpStatusCode.OK)
				{
					var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
					throw new ApplicationException(message);
				}

				using (var responseStream = response.GetResponseStream())
				{
					if (responseStream != null)
					{
						using (var reader = new StreamReader(responseStream))
						{
							responseValue = reader.ReadToEnd();
						}
					}
				}

				return responseValue;
			}
		}
	}
}