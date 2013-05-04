using System;
using System.Data.Services.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using RetryPolicies = DataAccess.Helpers.RetryPolicies;

namespace DataAccess.Exceptions
{
	internal static class ExceptionHandling
	{
		public static Exception MapToDataAccessException(Exception exception, Guid trialSequence, string message)
		{
			var errorCode = "N/A";

			var serverException = exception as StorageException;
			if (serverException != null)
			{
				errorCode = serverException.ErrorCode.ToString();
			}

			var dataServiceRequestException = exception as DataServiceRequestException;
			if (dataServiceRequestException != null)
			{
				errorCode = RetryPolicies.GetErrorCode(dataServiceRequestException);
			}

			var dataServiceQueryException = exception as DataServiceQueryException;
			if (dataServiceQueryException != null)
			{
				errorCode = RetryPolicies.GetErrorCode(dataServiceQueryException);
			}

			return new StorageDataAccessException(exception, trialSequence, message, errorCode);
		}

		public static bool Handle(
			Exception exception, TableServiceContext context, DataServiceResponse serviceResponse, out Exception newException)
		{
			newException = null;
			if (exception is DataServiceRequestException)
			{
				return Handle((DataServiceRequestException)exception, context, serviceResponse, out newException);
			}

			return false;
		}

		public static bool Handle(
			DataServiceRequestException exception, TableServiceContext context, DataServiceResponse serviceResponse, out Exception newException)
		{
			string errorCode;
			int? commandIndex;
			string errorMessage;
			ExceptionParser.ParseErrorDetails(exception.InnerException.Message, out errorCode, out commandIndex, out errorMessage);

			return CheckForEntityAlreadyExists(errorCode, context, commandIndex, errorMessage, out newException);
		}

		private static bool CheckForEntityAlreadyExists(
			string errorCode, TableServiceContext context, int? commandIndex, string errorMessage, out Exception newException)
		{
			newException = null;
			if (ExceptionParser.IsErrorStringMatch(errorCode, TableErrorCodeStrings.EntityAlreadyExists))
			{
				object entity = null;
				if (commandIndex.HasValue)
				{
					entity = context.Entities[commandIndex.Value].Entity;
				}

				newException = new EntityAlreadyExistsException(errorMessage, entity, Guid.NewGuid(), FrontendDataAccessAzurestorage);

				return true;
			}

			return false;
		}

		private const string FrontendDataAccessAzurestorage = "DataAccess.AzureStorage";
	}
}