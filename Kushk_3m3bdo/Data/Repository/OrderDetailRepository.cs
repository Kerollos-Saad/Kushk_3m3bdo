using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository
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
