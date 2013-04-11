using System;
using DataAccess.Exceptions;
using MediaRepositoryWebRole.Faults;

namespace MediaRepositoryWebRole
{
	public abstract class ServiceBase
	{
		protected ServiceBase()
		{
			_exceptionShield = new ExceptionShield();
		}

		protected TResponse ProcessWithExceptionShield<TResponse>(Func<TResponse> func)
		{
			return _exceptionShield.Process(func);
		}

		protected void ProcessWithExceptionShield(Action func)
		{
			_exceptionShield.Process(func);
		}

		private readonly IExceptionShield _exceptionShield;
	}
}