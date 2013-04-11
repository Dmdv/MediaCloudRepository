using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	public class UserRepository : RepositoryBase<User>, IUserRepository
	{
		public UserRepository(ITableStorageProvider<User> tableStorage)
			: base(tableStorage)
		{
		}

		public Task Add(User user)
		{
			return AddInternal(user);
		}

		public Task Add(string name, string password)
		{
			return AddInternal(new User(name, password));
		}

		public User Find(string name, string password, Guid? userId)
		{
			return 
				Get(DefaultSearchKey(new User(name, password) {UserId = userId}))
				.Result
				.SingleOrDefault(x => x.Password == password);
		}

		public bool IsExist(string name, string password, Guid? userId)
		{
			return Find(name, password, userId) != null;
		}
	}
}