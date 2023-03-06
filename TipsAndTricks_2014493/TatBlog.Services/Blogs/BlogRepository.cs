using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs
{
	public class BlogRepository : IBlogRepository
	{
		private readonly BlogDbContext _context;
		public BlogRepository(BlogDbContext context) 
		{
			_context = context;
		}
		public async Task<Post> GetPostAsync(
			int year,
			int mouth,
			string slug,
			CancellationToken cancellationToken = default) 
		{
			/*throw new NotImplementedException();*/
			IQueryable<Post> postsQuery = _context.Set<Post>()
				.Include(x => x.Category)
				.Include(x => x.Author);
			if (year >0)
			{
				postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
			}
			if (mouth > 0)
			{
				postsQuery = postsQuery.Where(x => x.PostedDate.Month == mouth);
			}
			if (!string.IsNullOrWhiteSpace(slug))
			{
				postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
			}
			return await postsQuery.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<IList<Post>> GetPopularArticlesAsync(
			int numPosts, CancellationToken cancellationToken = default)
		{  //throw new NotImplementedException();
			return await _context.Set<Post>()
				.Include(x => x.Author)
				.Include(x => x.Category)
				.OrderByDescending(p => p.ViewCount)
				.Take(numPosts)
				.ToListAsync(cancellationToken);
		}

		public async Task<bool> IsPostSlugExistedAsync(
			int postId,
			string slug,
			CancellationToken cancellationToken = default)
		{
			//throw new NotImplementedException();
			return await _context.Set<Post>()
				.AnyAsync(x => x.Id != postId && x.UrlSlug == slug,
				cancellationToken);
		}
		public async Task IncreaseViewCountAsync(
			int postId,
			CancellationToken cancellationToken = default)
		{
			//throw new NotImplementedException();
			await _context.Set<Post>()
				.Where(x => x.Id == postId)
				.ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1),cancellationToken);
		}
		public async Task<IList<CategoryItem>> GetCategoriesAsync(
			bool showOnMenu = false,
			CancellationToken cancellationToken = default)
		{
			IQueryable<Category> categories = _context.Set<Category>();
			if (showOnMenu)
			{
				categories = categories.Where(x => x.ShowOnMenu);
			}
			return await categories
				.OrderBy(x => x.Name)
				.Select(x => new CategoryItem()
				{
					Id = x.Id,
					Name = x.Name,
					UrlSlug = x.UrlSlug,
					Description = x.Description,
					ShowOnMenu = x.ShowOnMenu,
					PostCount = x.Posts.Count(p => p.Published)

				})
				.ToListAsync(cancellationToken);
		}
		public async Task<IPagedList<TagItem>> GetPageTagsAsync(
			IPagingParams pagingParams,
			CancellationToken cancellationToken = default)
		{
			var tagQuery = _context.Set<Tag>()
				.Select(x => new TagItem()
				{
					Id = x.Id,
					Name = x.Name,
					UrlSlug = x.UrlSlug,
					Description = x.Description,
					PostCount = x.Posts.Count(p => p.Published)
				});
			return await tagQuery
				.ToPagedListAsync(pagingParams, cancellationToken);
		}

		public Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
		{
			var tagQuery = _context.Set<Tag>()
				.Select(x => new TagItem()
				{
					Id = x.Id,
					Name = x.Name,
					UrlSlug = x.UrlSlug,
					Description = x.Description,
					PostCount = x.Posts.Count(p => p.Published)
				});
			return  tagQuery
				.ToPagedListAsync(pagingParams, cancellationToken);

		}
		public Task<Tag> FundTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
		{
			return _context.Set<Tag>()
				.Where(x => x.UrlSlug == slug)
				.FirstOrDefaultAsync(cancellationToken);
		}
		public async Task<IList<TagItem>> GetAllTagAsync(CancellationToken cancellationToken = default)
		{
			IQueryable<Tag> tags = _context.Set<Tag>();
			return await tags
				.OrderBy(x => x.Name)
				.Select(x => new TagItem()
				{
					Id = x.Id,
					Name = x.Name,
					UrlSlug = x.UrlSlug,
					Description = x.Description,
					PostCount = x.Posts.Count(p => p.Published)
				}).ToListAsync(cancellationToken);
		}
		//Xoá 1 thẻ theo mã cho trước 
		public class NotFountException : Exception 
		{
			public NotFountException(string entityName, int entityId)
				:base($"Entity '{entityName} with ID '{entityId}' was not found.")
			{

			}
			
		}
		public Task DeleteTag(int id, CancellationToken cancellationToken = default)
		{
			var TagDelete = _context.Set<Tag>().SingleOrDefault(x => x.Id == id);
			if (TagDelete != null)
			{
				_context.Tags.Remove(TagDelete);
				_context.SaveChanges();
				Console.WriteLine("Xoa the thanh cong");
			}
			else
			{
				Console.WriteLine("khong tim thay the de xoa");
			}
			return Task.CompletedTask;

		}
		//Tìm một chuyên mục (Category) theo tên định danh (slug). 
		public async Task<Category> FundCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
		{
			return await _context.Set<Category>()
				.Where(x => x.UrlSlug == slug)
				.FirstOrDefaultAsync(cancellationToken);
		}
		//Tìm một chuyên mục theo mã số cho trước
		public async Task<CategoryItem> FundCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _context.Set<Category>()
				.Where(x => x.Id == id)
				.Select(x => new CategoryItem()
				{
					Id = x.Id,
					Name = x.Name,
					UrlSlug = x.UrlSlug,
					Description = x.Description,
					PostCount = x.Posts.Count(x => x.Published)
				}).FirstOrDefaultAsync(cancellationToken);
		}
		//Thêm hoặc cập nhật một chuyên mục/chủ đề. 
		  public async Task AddCategory(string name, string urlslug, string description, CancellationToken cancellationToken = default)
		{
			_context.Categories.Add(new Category()
			{
				Name = name,
				UrlSlug = urlslug,
				Description = description,
				ShowOnMenu = true
			});
			Console.WriteLine("Them chuyen muc thanh cong!\n");
			Console.WriteLine("".PadRight(80, '-'));
			await _context.SaveChangesAsync(cancellationToken);
		}

	}
}
