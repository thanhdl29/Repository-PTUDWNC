using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs
{
	public class CategoryRepository	: ICategoryRepository
	{
		private readonly BlogDbContext _context;
		private readonly IMemoryCache _memoryCache;

		public CategoryRepository(BlogDbContext context, IMemoryCache memoryCache)
		{
			_context = context;
			_memoryCache = memoryCache;
		}

		public async Task<Category> GetCategoryBySlugAsync(
			string slug, CancellationToken cancellationToken = default)
		{
			return await _context.Set<Category>()
				.FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
		}

		public async Task<Category> GetCachedCategoryBySlugAsync(
			string slug, CancellationToken cancellationToken = default)
		{
			return await _memoryCache.GetOrCreateAsync(
				$"category.by-slug.{slug}",
				async (entry) =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
					return await GetCategoryBySlugAsync(slug, cancellationToken);
				});
		}

		public async Task<Category> GetCategoryByIdAsync(int categoryId)
			
		{
			return await _context.Set<Category>().FindAsync(categoryId);
		}

		public async Task<Category> GetCachedCategoryByIdAsync(int categoryId)
		{
			return await _memoryCache.GetOrCreateAsync(
				$"category.by-id.{categoryId}",
				async (entry) =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
					return await GetCategoryByIdAsync(categoryId);
				});
		}

		public async Task<IList<CategoryItem>> GetCategorysAsync(
			CancellationToken cancellationToken = default)
		{
			return await _context.Set<Category>()
				.OrderBy(a => a.Name)
				.Select(a => new CategoryItem()
				{
					Id = a.Id,
					Name = a.Name,
					Description = a.Description,
					ShowOnMenu = a.ShowOnMenu,
					UrlSlug = a.UrlSlug,
					PostCount = a.Posts.Count(p => p.Published)
				})
				.ToListAsync(cancellationToken);
		}

		public async Task<IPagedList<CategoryItem>> GetPagedCategorysAsync(
			IPagingParams pagingParams,
			string name = null,
			CancellationToken cancellationToken = default)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				return await _context.Set<Category>()
				.AsNoTracking()
				//.Where(!string.IsNullOrWhiteSpace(name), 
				//x => x.FullName.Contains(name))
				.Where(
					x => x.Name.Contains(name))
				.Select(a => new CategoryItem()
				{
					Id = a.Id,
					Name = a.Name,
					Description= a.Description,
					ShowOnMenu = a.ShowOnMenu,
					UrlSlug = a.UrlSlug,
					PostCount = a.Posts.Count(p => p.Published)
				})
				.ToPagedListAsync(pagingParams, cancellationToken);
			}
			else
			{
				return null;
			}

		}

		public async Task<IPagedList<T>> GetPagedCategorysAsync<T>(
			Func<IQueryable<Category>, IQueryable<T>> mapper,
			IPagingParams pagingParams,
			string name = null,
			CancellationToken cancellationToken = default)
		{
			var categoryQuery = _context.Set<Category>().AsNoTracking();

			if (!string.IsNullOrEmpty(name))
			{
				categoryQuery = categoryQuery.Where(x => x.Name.Contains(name));
			}

			return await mapper(categoryQuery)
				.ToPagedListAsync(pagingParams, cancellationToken);
		}

		public async Task<bool> AddOrUpdateAsync(
			Category category, CancellationToken cancellationToken = default)
		{
			if (category.Id > 0)
			{
				_context.Categories.Update(category);
				_memoryCache.Remove($"category.by-id.{category.Id}");
			}
			else
			{
				_context.Categories.Add(category);
			}

			return await _context.SaveChangesAsync(cancellationToken) > 0;
		}

		public async Task<bool> DeleteCategoryAsync(
			int categoryId, CancellationToken cancellationToken = default)
		{
			return await _context.Categories
				.Where(x => x.Id == categoryId)
				.ExecuteDeleteAsync(cancellationToken) > 0;
		}

		public async Task<bool> IsCategorySlugExistedAsync(
			int categoryId,
			string slug,
			CancellationToken cancellationToken = default)
		{
			return await _context.Categories
				.AnyAsync(x => x.Id != categoryId && x.UrlSlug == slug, cancellationToken);
		}

		
	}
}
