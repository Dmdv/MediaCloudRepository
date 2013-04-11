using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
	public class GenericAsyncEnumerator<T> : IAsyncEnumerator<T>
	{
		public GenericAsyncEnumerator(Func<CancellationToken, Task<bool>> moveNext, Func<T> current, Action dispose)
		{
			_moveNext = moveNext;
			_current = current;
			_dispose = dispose;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_dispose();
			}
		}

		public Task<bool> MoveNext(CancellationToken cancellationToken)
		{
			if (_disposed)
			{
				return TaskHelpers.FromResult(false);
			}

			return _moveNext(cancellationToken);
		}

		public T Current
		{
			get { return _current(); }
		}

		private readonly Func<T> _current;
		private readonly Action _dispose;
		private readonly Func<CancellationToken, Task<bool>> _moveNext;
		private bool _disposed;
	}
}