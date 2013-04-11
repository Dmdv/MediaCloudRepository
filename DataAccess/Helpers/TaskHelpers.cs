using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
	/// <summary>
	/// Расширение для обработки исключений, возникающих в тасках.
	/// </summary>
	public static class TaskHelpers
	{
		public static Task<TResult> FromResult<TResult>(TResult result)
		{
			return Task.FromResult(result);
		}

		public static void Handle<T, TResult>(this Task<T> task, TaskCompletionSource<TResult> tcs, Action<T> success)
		{
			task.Handle(success, ex => tcs.TrySetException(ex), () => tcs.TrySetCanceled());
		}

		public static Task<T> HandleSuccess<T>(this Task<T> task, Action<T> success)
		{
			return task.ContinueWith(
				t =>
					{
						if (!t.IsFaulted || !t.IsCanceled)
						{
							success(t.Result);
						}

						return t;
					},
				TaskContinuationOptions.ExecuteSynchronously).Unwrap();
		}

		public static Task HandleSuccess(this Task task, Action success)
		{
			return task.ContinueWith(
				t =>
					{
						if (!t.IsFaulted || !t.IsCanceled)
						{
							success();
						}

						return t;
					},
				TaskContinuationOptions.ExecuteSynchronously).Unwrap();
		}

		public static Task<T> HandleException<T>(this Task<T> task, Action<AggregateException> error)
		{
			return task.ContinueWith(
				t =>
					{
						if (t.IsFaulted)
						{
							error(t.Exception);
						}

						return t;
					},
				TaskContinuationOptions.ExecuteSynchronously).Unwrap();
		}

		public static Task HandleException(this Task task, Action<AggregateException> error)
		{
			return task.ContinueWith(
				t =>
					{
						if (t.IsFaulted)
						{
							error(t.Exception);
						}

						return t;
					},
				TaskContinuationOptions.ExecuteSynchronously).Unwrap();
		}

		public static void Handle<T>(this Task<T> task, Action<T> success, Action<AggregateException> error, Action canceled)
		{
			if (task.IsFaulted)
			{
				error(task.Exception);
			}
			else if (task.IsCanceled)
			{
				canceled();
			}
			else if (task.IsCompleted)
			{
				success(task.Result);
			}
		}

		public static Task<T> Finally<T>(this Task<T> task, Action action)
		{
			task.ContinueWith(t => action(), TaskContinuationOptions.ExecuteSynchronously);
			return task;
		}

		/// <summary>
		/// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		public static Task<TResult> FromError<TResult>(Exception exception)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exception);
			return tcs.Task;
		}

		/// <summary>
		/// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
		/// </summary>
		internal static Task<TResult> FromErrors<TResult>(IEnumerable<Exception> exceptions)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exceptions);
			return tcs.Task;
		}

		/// <summary>
		/// Returns a canceled Task of the given type. The task is completed, IsCanceled = True, IsFaulted = False.
		/// </summary>
		internal static Task<TResult> Canceled<TResult>()
		{
			return CancelCache<TResult>.Value;
		}

		#region Nested type: CancelCache

		/// <summary>
		/// This class is a convenient cache for per-type canceled tasks
		/// </summary>
		private static class CancelCache<TResult>
		{
			public static readonly Task<TResult> Value = GetCancelledTask();

			private static Task<TResult> GetCancelledTask()
			{
				var tcs = new TaskCompletionSource<TResult>();
				tcs.SetCanceled();
				return tcs.Task;
			}
		}

		#endregion
	}
}