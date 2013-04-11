using System.IO;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
	public interface IBlobRepository
	{
		Task SaveBlob(string containerName, string fileName, Stream stream);
	}
}