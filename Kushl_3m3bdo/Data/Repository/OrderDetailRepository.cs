using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository
{
	public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderDetailRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}
	}
}
