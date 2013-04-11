using System;
using System.Threading.Tasks;
using MediaRepositoryWebRole.Contracts;
using MediaRepositoryWebRole.Data;

namespace DataAccess.Test.Extensions
{
	public static class AsyncExtension
	{
		public static Task CreateUserAsync(this IUserManager userManager, User user)
		{
			return Task.Factory.FromAsync(userManager.BeginCreateUser, userManager.EndCreateUser, user, null);
		}
	}
}