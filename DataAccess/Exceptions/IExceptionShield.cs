using System;

namespace DataAccess.Exceptions
{
	public interface IExceptionShield
	{
		T Process<T>(Func<T> func);

		void Process(Action func);
	}
}