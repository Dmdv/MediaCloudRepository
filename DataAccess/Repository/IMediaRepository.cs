using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	public interface IMediaRepository
	{
		Task Add(MediaFile mediaFile);
		Task Delete(MediaFile mediaFile);
		bool IsExist(MediaFile mediaFile);
	}
}