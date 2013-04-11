using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Helpers;
using DataAccess.Repository;

namespace DataAccess.BusinessLogic
{
	/// <summary>
	/// Управляет пользователями.
	/// </summary>
	public class UserManager
	{
		private readonly UserRepository _userRepository;

		public UserManager(ITableStorageProvider<User> userContext)
		{
			_userRepository = new UserRepository(userContext);
		}

		/// <summary>
		/// Создает пользователя.
		/// </summary>
		public Task CreateUser(User user)
		{
			Guard.CheckNotNull(user, "user");
			return _userRepository.Add(user).HandleException(ex => ex.GetBaseException());
		}
	}
}