using System;
using System.ServiceModel;
using DataAccess.Exceptions;
using DataAccess.Extensions;
using MediaRepositoryWebRole.Extensions;

namespace MediaRepositoryWebRole.Faults
{
	public class ExceptionShield : IExceptionShield
	{
		public T Process<T>(Func<T> func)
		{
			var traceId = Guid.NewGuid();
			try
			{
				try
				{
					return func();
				}
				catch (AggregateException exception)
				{
					WriteInfoLog("AggregateException: {0}".FormatString(exception), traceId);
					if (exception.InnerExceptions.Count != 1)
					{
						WriteErrorLog(
							"Unexpected count {0} of inner exceptions - should be only one".FormatString(exception.InnerExceptions.Count),
							traceId);
					}

					var innerException = exception.InnerExceptions[0];
					if (innerException is AggregateException)
					{
						WriteErrorLog("Unexpected AggregateException as first inner exception", traceId);
					}

					throw innerException;
				}
			}
			catch (FaultException exception)
			{
				WriteInfoLog("FaultException: {0}".FormatString(exception), traceId);
				throw;
			}
			catch (UserNotFoundException exception)
			{
				WriteInfoLog("User {0} not found".FormatString(exception.UserId), exception.TraceId);
				throw new UserNotFoundFault(exception.UserId).ToFaultException();
			}
			catch (UserAlreadyExistsException exception)
			{
				WriteInfoLog("User {0} already exists".FormatString(exception.UserId), exception.TraceId);
				throw new UserAlreadyExistsFault(exception.UserId).ToFaultException();
			}
			catch (ArgumentException exception)
			{
				WriteErrorLog("Argument exception: {0}".FormatString(exception), traceId);
				throw new InternalServerErrorFault(InternalServerErrorFaultErrorCode.ArgumentException, traceId).ToFaultException();
			}
			catch (DeviceNotFoundException exception)
			{
				WriteWarningLog("Device not found. DeviceId: {0}".FormatString(exception.DeviceId), exception.TraceId);
				throw new DeviceNotFoundFault(exception.DeviceId.ToString(), exception.TraceId).ToFaultException();
			}
			catch (MediaAlreadyExistsException exception)
			{
				WriteWarningLog("Media file already exists. Filename: {0}".FormatString(exception.FileName), exception.TraceId);
				throw new MediaAlreadyExistsFault(exception.DeviceId, exception.FileName, exception.TraceId).ToFaultException();
			}
			catch (EntityAlreadyExistsException ex)
			{
				WriteWarningLog("Entity already exists.".FormatString(ex.Message), ex.TraceId);
				throw new EntityAlreadyExistsFault(ex.Message, ex.Entity).ToFaultException();
			}
			catch (Exception exception)
			{
				WriteErrorLog("Unexpected error: {0}".FormatString(exception.ToString()), traceId);
				throw new InternalServerErrorFault(InternalServerErrorFaultErrorCode.Unknown, traceId).ToFaultException();
			}
		}

		public void Process(Action func)
		{
			Process(() =>
				        {
					        func();
					        return true;
				        });
		}

		private static void WriteInfoLog(string message, Guid traceId)
		{
		}

		private static void WriteWarningLog(string message, Guid traceId)
		{
		}

		private static void WriteErrorLog(string message, Guid traceId)
		{
		}
	}
}