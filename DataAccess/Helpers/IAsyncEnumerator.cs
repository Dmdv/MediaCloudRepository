using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
	public interface IAsyncEnumerator<out T> : IDisposable
	{
		T Current { get; }
		Task<bool> MoveNext(CancellationToken cancellationToken);
	}
}