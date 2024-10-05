using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository
{
	public class FavouriteRepository : GenericRepository<Favourite>, IFavouriteRepository
	{
		private readonly ApplicationDbContext _context;
		public FavouriteRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}
	}
}
