using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Helpers;

namespace DataAccess.Exceptions
{
	public static class TaskExtensions
	{
		public static Task Delay(TimeSpan dueTime)
		{
			var num = (long)dueTime.TotalMilliseconds;
			Guard.CheckTrue(!(num < -1L || num > int.MaxValue), () => new ArgumentOutOfRangeException("dueTime"));

			var tcs = new TaskCompletionSource<bool>();
			var timer = new Timer(self =>
				                      {
					                      ((Timer)self).Dispose();
					                      tcs.TrySetResult(true);
				                      });
			timer.Change(num, -1);
			return tcs.Task;
		}

		public static IAsyncResult AsAsyncResult(this Task task, AsyncCallback callback, object state)
		{
			Guard.CheckNotNull(task, "task");
			Guard.CheckTrue(task.Status != TaskStatus.Created, () => new InvalidOperationException("TaskNotStarted"));

			var tcs = new TaskCompletionSource<object>(state);
			task.ContinueWith(
				t =>
					{
						if (t.IsFaulted)
						{
							Debug.Assert(t.Exception != null, "t.Exception != null");
							tcs.TrySetException(t.Exception.InnerExceptions);
						}
						else if (t.IsCanceled)
						{
							tcs.TrySetCanceled();
						}
						else
						{
							tcs.TrySetResult(null);
						}

						if (callback != null)
						{
							callback(tcs.Task);
						}
					},
				TaskContinuationOptions.ExecuteSynchronously);
			return tcs.Task;
		}

		public static IAsyncResult AsAsyncResult<T>(this Task<T> task, AsyncCallback callback, object state)
		{
			Guard.CheckNotNull(task, "task");
			Guard.CheckTrue(task.Status != TaskStatus.Created, () => new InvalidOperationException("TaskNotStarted"));

			var tcs = new TaskCompletionSource<T>(state);
			task.ContinueWith(
				t =>
					{
						if (t.IsFaulted)
						{
							Debug.Assert(t.Exception != null, "t.Exception != null");
							tcs.TrySetException(t.Exception.InnerExceptions);
						}
						else if (t.IsCanceled)
						{
							tcs.TrySetCanceled();
						}
						else
						{
							tcs.TrySetResult(t.Result);
						}

						if (callback != null)
						{
							callback(tcs.Task);
						}
					},
				TaskContinuationOptions.ExecuteSynchronously);
			return tcs.Task;
		}

		public static Task<bool> UsingEnumerator(this Task<bool> task, IDisposable disposable)
		{
			task.ContinueWith(
				t =>
					{
						if (t.IsFaulted || t.IsCanceled || !t.Result)
						{
							disposable.Dispose();
						}
					},
				TaskContinuationOptions.ExecuteSynchronously);

			return task;
		}
	}
}