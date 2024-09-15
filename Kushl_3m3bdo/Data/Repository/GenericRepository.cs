using Kushl_3m3bdo.Data.Repository.IRepository;

namespace Kushl_3m3bdo.Data.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _context;

		public GenericRepository(ApplicationDbContext context)
		{
			this._context = context;
		}

		public T GetById(int id)
		{
			return _context.Set<T>().Find(id);
		}
	}
}
