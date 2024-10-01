using System.Linq.Expressions;
using Kushk_3m3bdo.Models.Consts;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		T GetById(int id);

		Task<T> GetByIdAsync(int id);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		IEnumerable<T> GetAll();

		Task<IEnumerable<T>> GetAllAsync();

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		IEnumerable<T> FindAllExpressionProp(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		Task<IEnumerable<T>> FindAllExpressionPropAsync(Expression<Func<T, bool>> Filter,
			string[] includeProperties = null);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		IEnumerable<T> FindAllExpressionRange(Expression<Func<T, bool>> Filter, int skip, int take);

		Task<IEnumerable<T>> FindAllExpressionRangeAsync(Expression<Func<T, bool>> Filter, int skip, int take);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		IEnumerable<T> FindAll(Expression<Func<T, bool>>? Filter, int? skip, int? take,
			string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

		Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? Filter = null, int? skip = null, int? take = null,
			string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		T? FindNullable(Expression<Func<T, bool>> Filter, string[] includeProperties = null);
		Task<T?> FindNullableAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		T Find(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		Task<T> FindAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		T Add(T entity);

		Task<T> AddAsync(T entity);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		IEnumerable<T> AddRange(IEnumerable<T> entities);

		Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		T Remove(T entity);

		Task<T> RemoveAsync(T entity);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		IEnumerable<T> RemoveRange(IEnumerable<T> entities);

		Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> entities);

		// ----------------------------------------------------------------------------------------------------
		// ----------------------------------------------------------------------------------------------------

		T RemoveById(int Id);

		Task<T> RemoveByIdAsync(int Id);
	}
}
