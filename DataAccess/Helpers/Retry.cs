using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using TaskExtensions = DataAccess.Exceptions.TaskExtensions;

namespace DataAccess.Helpers
{
	internal static class Retry
	{
		public static Task<T> Do<T>(Func<T> action, RetryPolicy retryPolicy, CancellationToken cancellationToken)
		{
			return Do(() => Task<T>.Factory.StartNew(action), retryPolicy, cancellationToken);
		}

		public static Task<T> Do<T>(Func<Task<T>> taskGetter, RetryPolicy retryPolicy, CancellationToken cancellationToken)
		{
			const int RetryCount = 0;
			var tcs = new TaskCompletionSource<T>();
			var task = tcs.Task;
			taskGetter()
				.ContinueWith(finishedTask => TaskRetry(tcs, taskGetter, finishedTask, cancellationToken, retryPolicy(), RetryCount));
			return task;
		}

		private static void TaskRetry<T>(
			TaskCompletionSource<T> taskCompletionSource,
			Func<Task<T>> taskGetter,
			Task<T> finishedTask,
			CancellationToken cancellationToken,
			ShouldRetry policy,
			int retryCount)
		{
			if (finishedTask.Exception != null)
			{
				TimeSpan delay;
				if (policy(retryCount, finishedTask.Exception.InnerExceptions[0], out delay))
				{
					retryCount++;

					cancellationToken.ThrowIfCancellationRequested();

					TaskExtensions.Delay(delay)
					              .ContinueWith(_ =>
					                            taskGetter()
						                            .ContinueWith(x =>
						                                          TaskRetry(taskCompletionSource, taskGetter, x, cancellationToken, policy, retryCount)));
					return;
				}

				taskCompletionSource.SetException(finishedTask.Exception.InnerExceptions);
				return;
			}

			taskCompletionSource.SetResult(finishedTask.Result);
		}
	}
}