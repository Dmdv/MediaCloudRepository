using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace DataAccess.Extensions
{
	/// <summary>
	/// This class contains a set of helper methods to facilitate some common operations on enumerables.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Functional equivalent to foreach operator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="action">Action to be performed for each element of the source collection</param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}

		public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> function)
		{
			foreach (var item in source)
			{
				await function(item);
			}
		}

		/// <summary>
		/// Creates new collection with a target object as its sole element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<T> Yield<T>(this T value)
		{
			return new[] { value };
		}

		[Pure]
		public static T[] YieldArray<T>(this T item)
		{
			return new[] { item };
		}
	}
}