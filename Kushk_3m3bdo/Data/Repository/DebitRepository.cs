using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository
{
	public class DebitRepository : GenericRepository<Debit>, IDebitRepository
	{
		private readonly ApplicationDbContext _context;

		public DebitRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

	}
}
