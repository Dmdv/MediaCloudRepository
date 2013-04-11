using System;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	public interface IUserRepository
	{
		Task Add(User user);
		Task Add(string name, string password);
		User Find(string name, string password, Guid? userId);
		bool IsExist(string name, string password, Guid? userId);
	}
}