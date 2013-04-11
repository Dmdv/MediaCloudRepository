using System;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	public interface IQueryRepository
	{
		Task Add(QueryHistory query);
		QueryHistory GetOld(Guid deviceId);
		Task Delete(QueryHistory query);
		void Delete(Guid deviceId);
		void Delete(Guid deviceId, string fileName);
		string IsExist(Guid deviceId);
	}
}