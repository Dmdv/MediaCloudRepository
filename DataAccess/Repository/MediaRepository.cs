using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Repository
{
	public class MediaRepository : RepositoryBase<MediaFile>, IMediaRepository
	{
		public MediaRepository(ITableStorageProvider<MediaFile> tableStorage) :
			base(tableStorage)
		{
		}

		public Task Add(MediaFile mediaFile)
		{
			return AddInternal(mediaFile);
		}

		public Task Delete(MediaFile mediaFile)
		{
			return DeleteInternal(mediaFile);
		}

		public bool IsExist(MediaFile mediaFile)
		{
			return Get(DefaultSearchKey(mediaFile)).Result.Count() != 0;
		}
	}
}