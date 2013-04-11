using System;

namespace DataAccess.Helpers
{
	public class GenericAsyncEnumerable<T> : IAsyncEnumerable<T>
	{
		public GenericAsyncEnumerable(Func<IAsyncEnumerator<T>> getEnumerator)
		{
			_getEnumerator = getEnumerator;
		}

		#region IAsyncEnumerable<T> Members

		public IAsyncEnumerator<T> GetEnumerator()
		{
			return _getEnumerator();
		}

		#endregion

		private readonly Func<IAsyncEnumerator<T>> _getEnumerator;
	}
}