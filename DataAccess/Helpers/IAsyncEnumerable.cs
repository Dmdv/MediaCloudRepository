namespace DataAccess.Helpers
{
	public interface IAsyncEnumerable<out T>
	{
		IAsyncEnumerator<T> GetEnumerator();
	}
}