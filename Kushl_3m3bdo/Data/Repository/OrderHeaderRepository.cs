using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository
{
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}
	}
}
