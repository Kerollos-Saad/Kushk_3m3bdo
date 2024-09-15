using System.Linq.Expressions;
using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models.Consts;
using Microsoft.EntityFrameworkCore;

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

		public async Task<T> GetByIdAsync(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public IEnumerable<T> GetAll()
		{
			return _context.Set<T>().ToList();
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public T FindExpression(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = _context.Set<T>();

			if(includeProperties != null)
				foreach (var property in includeProperties)
					query = query.Include(property);
			
			return query.SingleOrDefault(Filter);
		}

		public async Task<T> FindExpressionAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			return await _context.Set<T>().SingleOrDefaultAsync(Filter);
		}

		public IEnumerable<T> FindAllExpression(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = _context.Set<T>();

			if (includeProperties != null)
				foreach (var property in includeProperties)
					query = query.Include(property);

			return query.Where(Filter).ToList();
		}

		public async Task<IEnumerable<T>> FindAllExpressionAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			return await _context.Set<T>().Where(Filter).ToListAsync();
		}

		public IEnumerable<T> FindAllExpression(Expression<Func<T, bool>> Filter, int skip, int take)
		{
			return _context.Set<T>().Where(Filter).Skip(skip).Take(take).ToList();
		}

		public async Task<IEnumerable<T>> FindAllExpressionAsync(Expression<Func<T, bool>> Filter, int skip, int take)
		{
			return await _context.Set<T>().Where(Filter).Skip(skip).Take(take).ToListAsync();
		}

		public IEnumerable<T> FindAll(Expression<Func<T, bool>>? Filter, int? skip, int? take,
			string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
		{
			IQueryable<T> query = _context.Set<T>();

			if (includeProperties != null)
				foreach (var property in includeProperties)
					query = query.Include(property);

			if (Filter!= null)
				query = query.Where(Filter);

			if(take.HasValue)
				query = query.Take(take.Value);

			if (skip.HasValue)
				query = query.Skip(skip.Value);

			if (orderBy != null)
			{
				if(orderByDirection ==  OrderBy.Ascending)
					query = query.OrderBy(orderBy);
				else
					query = query.OrderByDescending(orderBy);
			}

			return query.ToList();
		}

		public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> Filter, int? skip, int? take, string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
		{
			IQueryable<T> query = _context.Set<T>();

			if (includeProperties != null)
				foreach (var property in includeProperties)
					query = query.Include(property);

			if (Filter != null)
				query = query.Where(Filter);

			if (take.HasValue)
				query = query.Take(take.Value);

			if (skip.HasValue)
				query = query.Skip(skip.Value);

			if (orderBy != null)
			{
				if (orderByDirection == OrderBy.Ascending)
					query = query.OrderBy(orderBy);
				else
					query = query.OrderByDescending(orderBy);
			}

			return await query.ToListAsync();
		}

		public T Add(T entity)
		{
			_context.Set<T>().Add(entity);
			_context.SaveChanges();

			return entity;
		}

		public async Task<T> AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
			await _context.SaveChangesAsync();

			return entity;
		}

		public IEnumerable<T> AddRange(IEnumerable<T> entities)
		{
			_context.Set<T>().AddRange(entities);
			_context.SaveChanges();

			return entities;
		}

		public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
		{
			await _context.Set<T>().AddRangeAsync(entities);
			await _context.SaveChangesAsync();

			return entities;
		}

		public T Remove(T entity)
		{
			_context.Set<T>().Remove(entity);
			_context.SaveChanges();

			return entity;
		}

		public async Task<T> RemoveAsync(T entity)
		{
			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();

			return entity;
		}

		public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
		{
			_context.Set<T>().RemoveRange(entities);
			_context.SaveChanges();

			return entities;
		}

		public async Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> entities)
		{
			_context.Set<T>().RemoveRange(entities);
			await _context.SaveChangesAsync();

			return entities;
		}
	}
}
