using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
	/// <summary>
	/// Расширение для создания асинхронного енумератора.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	internal sealed class AsyncEnumerableToAsyncTransformer<TResult, TValue> : IAsyncEnumerable<TResult>
	{
		public AsyncEnumerableToAsyncTransformer(
			IAsyncEnumerable<IEnumerable<TValue>> sourceAsyncEnumerable,
			Func<TValue,
				TResult> converter)
		{
			_sourceAsyncEnumerable = sourceAsyncEnumerable;
			_converter = converter;
		}

		#region IAsyncEnumerable<TResult> Members

		public IAsyncEnumerator<TResult> GetEnumerator()
		{
			return new AsyncEnumerableToAsyncEnumerator(_sourceAsyncEnumerable.GetEnumerator(), _converter);
		}

		#endregion

		private readonly Func<TValue, TResult> _converter;
		private readonly IAsyncEnumerable<IEnumerable<TValue>> _sourceAsyncEnumerable;

		#region Nested type: AsyncEnumerableToAsyncEnumerator

		private sealed class AsyncEnumerableToAsyncEnumerator : IAsyncEnumerator<TResult>
		{
			public AsyncEnumerableToAsyncEnumerator(
				IAsyncEnumerator<IEnumerable<TValue>> asyncEnumerator,
				Func<TValue, TResult> converter)
			{
				_asyncEnumerator = asyncEnumerator;
				_converter = converter;
			}

			#region IAsyncEnumerator<TResult> Members

			public TResult Current
			{
				get { return _converter(_myCurrent); }
			}

			public Task<bool> MoveNext(CancellationToken cancellationToken)
			{
				return _syncEnumerator == null ? MoveAsync(cancellationToken) : MoveSync(cancellationToken);
			}

			public void Dispose()
			{
				try
				{
					if (_syncEnumerator != null)
					{
						_syncEnumerator.Dispose();
					}
				}
				finally
				{
					_asyncEnumerator.Dispose();
				}
			}

			#endregion

			private Task<bool> MoveSync(CancellationToken cancellationToken)
			{
				try
				{
					if (_syncEnumerator.MoveNext())
					{
						_myCurrent = _syncEnumerator.Current;
						return CachedTrueTask;
					}

					_syncEnumerator.Dispose();
					_syncEnumerator = null;
				}
				catch (Exception exception)
				{
					var taskSource = new TaskCompletionSource<bool>();
					taskSource.SetException(exception);
					return taskSource.Task;
				}

				return MoveAsync(cancellationToken);
			}

			private async Task<bool> MoveAsync(CancellationToken cancellationToken)
			{
				if (await _asyncEnumerator.MoveNext(cancellationToken))
				{
					_syncEnumerator = _asyncEnumerator.Current.GetEnumerator();
					return await MoveNext(cancellationToken);
				}

				return false;
			}

			// ReSharper disable StaticFieldInGenericType
			private static readonly Task<bool> CachedTrueTask = TaskHelpers.FromResult(true);
			// ReSharper restore StaticFieldInGenericType
			private readonly IAsyncEnumerator<IEnumerable<TValue>> _asyncEnumerator;
			private readonly Func<TValue, TResult> _converter;
			private TValue _myCurrent;
			private IEnumerator<TValue> _syncEnumerator;
		}

		#endregion
	}
}