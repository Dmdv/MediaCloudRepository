using System;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using DataAccess.Exceptions;
using Microsoft.WindowsAzure.StorageClient;

namespace DataAccess.Helpers
{
	/// <summary>
	/// 	Azure retry policies for corner-situation and server errors.
	/// </summary>
	public class RetryPolicies
	{
		internal RetryPolicies()
		{
		}

		/// <summary>
		/// 	Retry policy for optimistic concurrency retrials.
		/// </summary>
		public ShouldRetry OptimisticConcurrency()
		{
			var random = new Random();

			return delegate(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
			{
				if (currentRetryCount >= 30)
				{
					retryInterval = TimeSpan.Zero;
					return false;
				}

				retryInterval = TimeSpan.FromMilliseconds(random.Next(Math.Min(10000, 10 + (currentRetryCount * currentRetryCount * 10))));
				return true;
			};
		}

		/// <summary>
		/// Retry policy to temporarily back off in case of transient Azure server errors, 
		/// system overload or in case the denial of service detection system thinks we're a too heavy user. 
		/// Blocks the thread while backing off to prevent further requests for a while (per thread).
		/// </summary>
		public ShouldRetry TransientServerErrorBackOff()
		{
			return delegate(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
			{
				if (currentRetryCount >= 30 || !TransientServerErrorExceptionFilter(lastException))
				{
					retryInterval = TimeSpan.Zero;
					return false;
				}

				// quadratic backoff, capped at 5 minutes
				var c = currentRetryCount + 1;
				retryInterval = TimeSpan.FromSeconds(Math.Min(300, c * c));
				return true;
			};
		}

		/// <summary>
		/// 	Similar to <see cref="TransientServerErrorBackOff" /> , yet the Table Storage comes with its own set or exceptions/.
		/// </summary>
		public ShouldRetry TransientTableErrorBackOff()
		{
			return delegate(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
			{
				var notTransient =
					!(TransientTableErrorExceptionFilter(lastException) || TransientServerErrorExceptionFilter(lastException));
				if (currentRetryCount >= 10 || notTransient)
				{
					retryInterval = TimeSpan.Zero;
					return false;
				}

				// quadratic backoff, capped at 60 sec
				var c = currentRetryCount + 1;
				retryInterval = TimeSpan.FromSeconds(Math.Min(60, c * 2));
				return true;
			};
		}

		/// <summary>
		/// 	Very patient retry policy to deal with container, queue or table instantiation that happens just after a deletion.
		/// </summary>
		public ShouldRetry SlowInstantiation()
		{
			return delegate(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
			{
				if (currentRetryCount >= 30 || !SlowInstantiationExceptionFilter(lastException))
				{
					retryInterval = TimeSpan.Zero;
					return false;
				}

				// linear backoff
				retryInterval = TimeSpan.FromMilliseconds(100 * currentRetryCount);
				return true;
			};
		}

		/// <summary>
		/// 	Limited retry related to MD5 validation failure.
		/// </summary>
		public ShouldRetry NetworkCorruption()
		{
			return delegate(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
			{
				if (currentRetryCount >= 3 || !NetworkCorruptionExceptionFilter(lastException))
				{
					retryInterval = TimeSpan.Zero;
					return false;
				}

				// no backoff, retry immediately
				retryInterval = TimeSpan.Zero;
				return true;
			};
		}

		/// <summary>
		/// 	Hack around lack of proper way of retrieving the error code through a property.
		/// </summary>
		public static string GetErrorCode(DataServiceRequestException ex)
		{
			string errorCode;
			int? commandIndex;
			string message;
			ExceptionParser.ParseErrorDetails(ex.InnerException.Message, out errorCode, out commandIndex, out message);

			return errorCode;
		}

		// HACK: just duplicating the other overload of 'GetErrorCode'

		/// <summary>
		/// 	Hack around lack of proper way of retrieving the error code through a property.
		/// </summary>
		public static string GetErrorCode(DataServiceQueryException ex)
		{
			string errorCode;
			int? commandIndex;
			string message;
			ExceptionParser.ParseErrorDetails(ex.InnerException.Message, out errorCode, out commandIndex, out message);

			return errorCode;
		}

		private static bool IsErrorCodeMatch(StorageException exception, params StorageErrorCode[] codes)
		{
			return exception != null
			       && codes.Contains(exception.ErrorCode);
		}

		private static bool IsErrorStringMatch(StorageException exception, params string[] errorStrings)
		{
			return exception != null && exception.ExtendedErrorInformation != null
			       && errorStrings.Contains(exception.ExtendedErrorInformation.ErrorCode);
		}

		private static bool TransientServerErrorExceptionFilter(Exception exception)
		{
			var serverException = exception as StorageServerException;
			if (serverException != null)
			{
				if (IsErrorCodeMatch(
					serverException,
					StorageErrorCode.ServiceInternalError,
					StorageErrorCode.ServiceTimeout))
				{
					return true;
				}

				if (IsErrorStringMatch(
					serverException,
					StorageErrorCodeStrings.InternalError,
					StorageErrorCodeStrings.ServerBusy,
					StorageErrorCodeStrings.OperationTimedOut))
				{
					return true;
				}

				return false;
			}

			var webException = exception as WebException;
			if (webException != null &&
			    (webException.Status == WebExceptionStatus.ConnectionClosed ||
			     webException.Status == WebExceptionStatus.ConnectFailure ||
			     webException.Status == WebExceptionStatus.Timeout))
			{
				return true;
			}

			var ioException = exception as IOException;
			if (ioException != null)
			{
				return true;
			}

			// HACK: StorageClient does not catch internal errors very well.
			// Hence we end up here manually catching exception that should have been correctly 
			// typed by the StorageClient:

			// System.Net.InternalException is internal, but uncaught on some race conditions.
			// We therefore assume this is a transient error and retry.
			var exceptionType = exception.GetType();
			if (exceptionType.FullName == "System.Net.InternalException")
			{
				return true;
			}

			return false;
		}

		private static bool TransientTableErrorExceptionFilter(Exception exception)
		{
			var storageClientException = exception as StorageClientException;
			if (storageClientException != null)
			{
				// случай когда лезем через проксю, и ответа никакого нет
				// текст взят из исходников Microsoft.WindowsAzure.StorageClient.dll
				const string UnexpectedInternalClientTimeoutError =
					"The operation has exceeded the default maximum time allowed for Windows Azure Table service operations.";
				if (storageClientException.ErrorCode == StorageErrorCode.None
				    && storageClientException.StatusCode == HttpStatusCode.Unused
				    && storageClientException.ExtendedErrorInformation == null
				    && storageClientException.InnerException == null
				    && string.Equals(
					    storageClientException.Message,
					    UnexpectedInternalClientTimeoutError,
					    StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			var dataServiceRequestException = exception as DataServiceRequestException;
			if (dataServiceRequestException != null)
			{
				if (ExceptionParser.IsErrorStringMatch(
					GetErrorCode(dataServiceRequestException),
					StorageErrorCodeStrings.InternalError,
					StorageErrorCodeStrings.ServerBusy,
					StorageErrorCodeStrings.OperationTimedOut,
					TableErrorCodeStrings.TableServerOutOfMemory))
				{
					return true;
				}
			}

			var dataServiceQueryException = exception as DataServiceQueryException;
			if (dataServiceQueryException != null)
			{
				if (ExceptionParser.IsErrorStringMatch(
					GetErrorCode(dataServiceQueryException),
					StorageErrorCodeStrings.InternalError,
					StorageErrorCodeStrings.ServerBusy,
					StorageErrorCodeStrings.OperationTimedOut,
					TableErrorCodeStrings.TableServerOutOfMemory))
				{
					return true;
				}
			}

			// The remote server returned an error: (500) Internal Server Error, or some timeout
			// The server should not timeout in theory (that's why there are limits and pagination)
			var webException = exception as WebException;
			if (webException != null &&
			    (webException.Status == WebExceptionStatus.ProtocolError ||
			     webException.Status == WebExceptionStatus.ConnectionClosed ||
			     webException.Status == WebExceptionStatus.ConnectFailure ||
			     webException.Status == WebExceptionStatus.Timeout))
			{
				return true;
			}

			var ioException = exception as IOException;
			if (ioException != null)
			{
				return true;
			}

			// HACK: StorageClient does not catch internal errors very well.
			// Hence we end up here manually catching exception that should have been correctly 
			// typed by the StorageClient:

			// System.Net.InternalException is internal, but uncaught on some race conditions.
			// We therefore assume this is a transient error and retry.
			var exceptionType = exception.GetType();
			if (exceptionType.FullName == "System.Net.InternalException")
			{
				return true;
			}

			return false;
		}

		private static bool SlowInstantiationExceptionFilter(Exception exception)
		{
			var storageException = exception as StorageClientException;

			// Blob Storage or Queue Storage exceptions
			// Table Storage may throw exception of type 'StorageClientException'
			if (storageException != null)
			{
				// 'client' exceptions reflect server-side problems (delayed instantiation)
				if (IsErrorCodeMatch(
					storageException,
					StorageErrorCode.ResourceNotFound,
					StorageErrorCode.ContainerNotFound))
				{
					return true;
				}

				if (IsErrorStringMatch(
					storageException,
					QueueErrorCodeStrings.QueueNotFound,
					QueueErrorCodeStrings.QueueBeingDeleted,
					StorageErrorCodeStrings.InternalError,
					StorageErrorCodeStrings.ServerBusy,
					TableErrorCodeStrings.TableServerOutOfMemory,
					TableErrorCodeStrings.TableNotFound,
					TableErrorCodeStrings.TableBeingDeleted))
				{
					return true;
				}
			}

			// Table Storage may also throw exception of type 'DataServiceQueryException'.
			var dataServiceException = exception as DataServiceQueryException;
			if (null != dataServiceException)
			{
				if (ExceptionParser.IsErrorStringMatch(
					GetErrorCode(dataServiceException),
					TableErrorCodeStrings.TableBeingDeleted,
					TableErrorCodeStrings.TableNotFound,
					TableErrorCodeStrings.TableServerOutOfMemory))
				{
					return true;
				}
			}

			return false;
		}

		private static bool NetworkCorruptionExceptionFilter(Exception exception)
		{
			// Upload MD5 mismatch
			var clientException = exception as StorageClientException;
			if (clientException != null
			    && clientException.ErrorCode == StorageErrorCode.BadRequest
			    && clientException.ExtendedErrorInformation != null
			    && clientException.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.InvalidHeaderValue
			    && clientException.ExtendedErrorInformation.AdditionalDetails["HeaderName"] == "Content-MD5")
			{
				// network transport corruption (automatic), try again
				return true;
			}

			return false;
		}
	}
}