using System.Linq.Expressions;
using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models.Consts;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Data.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly ApplicationDbContext _context;
		internal DbSet<T> DbSet;

		public GenericRepository(ApplicationDbContext context)
		{
			this._context = context;
			this.DbSet = _context.Set<T>();
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public T GetById(int id)
		{
			return DbSet.Find(id);
		}

		public async Task<T> GetByIdAsync(int id)
		{
			return await DbSet.FindAsync(id);
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public IEnumerable<T> GetAll()
		{
			return DbSet.ToList();
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await DbSet.ToListAsync();
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public IEnumerable<T> FindAllExpressionProp(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>(); 
			foreach (var property in includeProperties)
					query = query.Include(property);

			return query.Where(Filter).ToList();
		}

		public async Task<IEnumerable<T>> FindAllExpressionPropAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>(); 
			foreach (var property in includeProperties)
					query = query.Include(property);

			return await query.Where(Filter).ToListAsync();
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public IEnumerable<T> FindAllExpressionRange(Expression<Func<T, bool>> Filter, int skip, int take)
		{
			return DbSet.Where(Filter).Skip(skip).Take(take).ToList();
		}

		public async Task<IEnumerable<T>> FindAllExpressionRangeAsync(Expression<Func<T, bool>> Filter, int skip, int take)
		{
			return await DbSet.Where(Filter).Skip(skip).Take(take).ToListAsync();
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public IEnumerable<T> FindAll(Expression<Func<T, bool>>? Filter, int? skip, int? take,
			string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided by (??) null-coalescing operator
			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
					query = query.Include(property);

			// Apply Filtering
			if (Filter != null)
				query = query.Where(Filter);

			// Apply ordering
			if (orderBy != null)
			{
				query = orderByDirection == OrderBy.Ascending
					? query.OrderBy(orderBy)
					: query.OrderByDescending(orderBy);
			}

			// Apply pagination
			if (skip.HasValue)
				query = query.Skip(skip.Value);
			if (take.HasValue)
				query = query.Take(take.Value);

			return query.ToList();
		}

		public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? Filter = null, int? skip = null, int? take = null, string[] includeProperties = null,
			Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
					query = query.Include(property);

			// Apply Filtering
			if (Filter != null)
				query = query.Where(Filter);

			// Apply ordering
			if (orderBy != null)
			{
				query = orderByDirection == OrderBy.Ascending
					? query.OrderBy(orderBy)
					: query.OrderByDescending(orderBy);
			}

			// Apply pagination
			if (skip.HasValue)
				query = query.Skip(skip.Value);
			if (take.HasValue)
				query = query.Take(take.Value);

			return await query.ToListAsync();
		}


		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public T? FindNullable(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
				query = query.Include(property);

			return query.SingleOrDefault(Filter);
		}

		public async Task<T?> FindNullableAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
				query = query.Include(property);

			return await query.SingleOrDefaultAsync(Filter);
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public T Find(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
				query = query.Include(property);

			// Apply filtering
			if (Filter != null)
				query = query.Where(Filter);

			return query.FirstOrDefault();
		}

		public async Task<T> FindAsync(Expression<Func<T, bool>> Filter, string[] includeProperties = null)
		{
			IQueryable<T> query = DbSet;

			// Include properties if provided
			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
				query = query.Include(property);

			// Apply filtering
			if (Filter != null)
				query = query.Where(Filter);

			return await query.FirstOrDefaultAsync();
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public T Add(T entity)
		{
			_context.Set<T>().Add(entity);

			return entity;
		}

		public async Task<T> AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);

			return entity;
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public IEnumerable<T> AddRange(IEnumerable<T> entities)
		{
			_context.Set<T>().AddRange(entities);

			return entities;
		}

		public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
		{
			await _context.Set<T>().AddRangeAsync(entities);

			return entities;
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public T Remove(T entity)
		{
			_context.Set<T>().Remove(entity);

			return entity;
		}

		public async Task<T> RemoveAsync(T entity)
		{
			_context.Set<T>().Remove(entity);

			return entity;
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
		{
			_context.Set<T>().RemoveRange(entities);

			return entities;
		}

		public async Task<IEnumerable<T>> RemoveRangeAsync(IEnumerable<T> entities)
		{
			_context.Set<T>().RemoveRange(entities);

			return entities;
		}

		// ---------------------------------------------------------------------------------------------------
		// ---------------------------------------------------------------------------------------------------

		public T RemoveById(int Id)
		{
			var entity = _context.Set<T>().Find(Id);

			_context.Set<T>().Remove(entity);

			return entity;
		}

		public async Task<T> RemoveByIdAsync(int Id)
		{
			var entity = await _context.Set<T>().FindAsync(Id);

			_context.Set<T>().Remove(entity);

			return entity;
		}
	}
}
