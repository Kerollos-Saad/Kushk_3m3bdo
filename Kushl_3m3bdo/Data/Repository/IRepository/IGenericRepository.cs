namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		T GetById(int id);
	}
}
