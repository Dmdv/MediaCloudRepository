using System.Collections.Generic;

namespace DataAccess.Repository
{
	/// <summary>
	/// Батч сохраняемых запросов.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Batch<T>
	{
		public Batch(IList<T> inserted = null, IList<T> updated = null, IList<T> deleted = null, IList<T> insertOrReplace = null)
		{
			Insert = inserted;
			Update = updated;
			Delete = deleted;
			InsertOrReplace = insertOrReplace;
		}

		public IList<T> Insert { get; set; }

		public IList<T> Update { get; set; }

		public IList<T> Delete { get; set; }

		public IList<T> InsertOrReplace { get; set; }
	}
}