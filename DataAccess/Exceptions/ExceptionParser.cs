using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DataAccess.Entities;
using DataAccess.Extensions;

namespace DataAccess.Exceptions
{
	internal static class ExceptionParser
	{
		public static void ParseErrorDetails(
			string exceptionMessage, out string errorCode, out int? commandIndex, out string errorMessage)
		{
			GetErrorInformation(exceptionMessage, out errorCode, out errorMessage);

			commandIndex = null;
			var indexOfSeparator = errorMessage.IndexOf(':');
			if (indexOfSeparator > 0)
			{
				int temp;
				if (int.TryParse(errorMessage.Substring(0, indexOfSeparator), out temp))
				{
					commandIndex = temp;
					errorMessage = errorMessage.Substring(indexOfSeparator + 1);
				}
			}
		}

		public static bool IsErrorStringMatch(string exceptionErrorString, params string[] errorStrings)
		{
			return errorStrings.Any(exceptionErrorString.OrdinalEqualsCs);
		}

		private static void GetErrorInformation(
			string xmlErrorMessage,
			out string errorCode,
			out string message)
		{
			message = null;
			errorCode = null;

			var xnErrorCode = XName.Get(
				"code",
				"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
			var xnMessage = XName.Get(
				"message",
				"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");

			using (var reader = new StringReader(xmlErrorMessage))
			{
				XDocument xDocument;
				try
				{
					xDocument = XDocument.Load(reader);
				}
				catch (XmlException)
				{
					// todo logging?
					return;
				}

				var errorCodeElement =
					xDocument.Descendants(xnErrorCode).FirstOrDefault();

				if (errorCodeElement == null)
				{
					return;
				}

				errorCode = errorCodeElement.Value;

				var messageElement =
					xDocument.Descendants(xnMessage).FirstOrDefault();

				if (messageElement != null)
				{
					message = messageElement.Value;
				}
			}
		}
	}
}