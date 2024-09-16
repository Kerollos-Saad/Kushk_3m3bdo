using System.Linq.Expressions;
using Kushl_3m3bdo.Models.Consts;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		T GetById(int id);

		Task<T> GetByIdAsync(int id);

		IEnumerable<T> GetAll();

		Task<IEnumerable<T>> GetAllAsync();

		T FindExpression(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		Task<T> FindExpressionAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		IEnumerable<T> FindAllExpression(Expression<Func<T, bool>> Filter, string[] includeProperties = null);

		Task<IEnumerable<T>> FindAllExpressionAsync(Expression<Func<T, bool>> Filter,
			string[] includeProperties = null);

		IEnumerable<T> FindAllExpression(Expression<Func<T, bool>> Filter, int skip, int take);

		Task<IEnumerable<T>> FindAllExpressionAsync(Expression<Func<T, bool>> Filter, int skip, int take);

		IEnumerable<T> FindAll(Expression<Func<T, bool>> Filter, int? skip, int? take,
			string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

		Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> Filter, int? skip, int? take,
			string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

		T Add(T entity);

		Task<T> AddAsync(T entity);

		IEnumerable<T> AddRange(IEnumerable<T> entities);

		Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

		T Remove(T entity);

		Task<T> RemoveAsync(T entity);

		IEnumerable<T> RemoveRange(IEnumerable<T> entities);

		Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> entities);

		T RemoveWithId(int Id);

		Task<T> RemoveWithIdAsync(int Id);
	}
}
