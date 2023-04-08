using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
	public class PostRepository	: IPostRepository
	{
		private readonly BlogDbContext _context;
		private readonly IMemoryCache _memoryCache;

		public PostRepository(BlogDbContext context, IMemoryCache memoryCache)
		{
			_context = context;
			_memoryCache = memoryCache;
		}

		public async Task<Post> GetPostBySlugAsync(
			string slug, CancellationToken cancellationToken = default)
		{
			return await _context.Set<Post>()
				.FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
		}

		public async Task<Post> GetCachedPostBySlugAsync(
			string slug, CancellationToken cancellationToken = default)
		{
			return await _memoryCache.GetOrCreateAsync(
				$"posts.by-slug.{slug}",
				async (entry) =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
					return await GetPostBySlugAsync(slug, cancellationToken);
				});
		}

		public async Task<Post> GetPostByIdAsync(int postId)

		{
			return await _context.Set<Post>().FindAsync(postId);
		}

		public async Task<Post> GetCachedPostByIdAsync(int postId)
		{
			return await _memoryCache.GetOrCreateAsync(
				$"posts.by-id.{postId}",
				async (entry) =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
					return await GetPostByIdAsync(postId);
				});
		}

		public async Task<IList<PostItem>> GetPostAsync(
			CancellationToken cancellationToken = default)
		{
			return await _context.Set<Post>()
				.OrderBy(a => a.Title)
				.Select(a => new PostItem()
				{
					Id = a.Id,
					Title = a.Title,
					ShortDescription = a.ShortDescription,
					Description = a.Description,
					UrlSlug = a.UrlSlug,
					Meta = a.Meta,
					ImageUrl = a.ImageUrl,
					ViewCount = a.ViewCount,
					Published = a.Published,
					PostedDate= a.PostedDate,
					ModifiedDate = a.ModifiedDate,

				})
				.ToListAsync(cancellationToken);
		}

		public async Task<IPagedList<PostItem>> GetPagedPostAsync(
			IPagingParams pagingParams,
			string title = null,
			CancellationToken cancellationToken = default)
		{
			if (!string.IsNullOrWhiteSpace(title))
			{
				return await _context.Set<Post>()
				.AsNoTracking()
				//.Where(!string.IsNullOrWhiteSpace(name), 
				//x => x.FullName.Contains(name))
				.Where(
					x => x.Title.Contains(title))
				.Select(a => new PostItem()
				{
					Id = a.Id,
					Title = a.Title,
					ShortDescription = a.ShortDescription,
					Description = a.Description,
					UrlSlug = a.UrlSlug,
					Meta = a.Meta,
					ImageUrl = a.ImageUrl,
					ViewCount = a.ViewCount,
					Published = a.Published,
					PostedDate = a.PostedDate,
					ModifiedDate = a.ModifiedDate,
				})
				.ToPagedListAsync(pagingParams, cancellationToken);
			}
			else
			{
				return null;
			}

		}

		public async Task<IPagedList<T>> GetPagedPostAsync<T>(
			Func<IQueryable<Post>, IQueryable<T>> mapper,
			IPagingParams pagingParams,
			string title = null,
			CancellationToken cancellationToken = default)
		{
			var postQuery = _context.Set<Post>().AsNoTracking();

			if (!string.IsNullOrEmpty(title))
			{
				postQuery = postQuery.Where(x => x.Title.Contains(title));
			}

			return await mapper(postQuery)
				.ToPagedListAsync(pagingParams, cancellationToken);
		}

		public async Task<bool> AddOrUpdateAsync(
			Post post, CancellationToken cancellationToken = default)
		{
			if (post.Id > 0)
			{
				_context.Posts.Update(post);
				_memoryCache.Remove($"post.by-id.{post.Id}");
			}
			else
			{
				_context.Posts.Add(post);
			}

			return await _context.SaveChangesAsync(cancellationToken) > 0;
		}

		public async Task<bool> DeletePostAsync(
			int postId, CancellationToken cancellationToken = default)
		{
			return await _context.Posts
				.Where(x => x.Id == postId)
				.ExecuteDeleteAsync(cancellationToken) > 0;
		}

		public async Task<bool> IsPostSlugExistedAsync(
			int postId,
			string slug,
			CancellationToken cancellationToken = default)
		{
			return await _context.Posts
				.AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
		}

		private IQueryable<Post> FilterPosts(PostQuery condition)
		{
			IQueryable<Post> posts = _context.Set<Post>()
				.Include(x => x.Category)
				.Include(x => x.Author)
				.Include(x => x.Tags);
			if (condition.Publishedonly)
			{
				posts = posts.Where(x => x.Published);
			}
			if (condition.NotPublished)
			{
				posts = posts.Where(x => !x.Published);
			}
			if (condition.CategoryId > 0)
			{
				posts = posts.Where(x => x.CategoryId == condition.CategoryId);
			}

			if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
			{
				posts = posts.Where(x => x.Category.UrlSlug == condition.CategorySlug);
			}

			if (condition.AuthorId > 0)
			{
				posts = posts.Where(x => x.AuthorId == condition.AuthorId);
			}

			if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
			{
				posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
			}

			if (!string.IsNullOrWhiteSpace(condition.TagSlug))
			{
				posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
			}

			if (!string.IsNullOrWhiteSpace(condition.Keyword))
			{
				posts = posts.Where(x => x.Title.Contains(condition.Keyword) ||
										 x.ShortDescription.Contains(condition.Keyword) ||
										 x.Description.Contains(condition.Keyword) ||
										 x.Category.Name.Contains(condition.Keyword) ||
										 x.Tags.Any(t => t.Name.Contains(condition.Keyword)));
			}

			if (condition.Year > 0)
			{
				posts = posts.Where(x => x.PostedDate.Year == condition.Year);
			}

			if (condition.Month > 0)
			{
				posts = posts.Where(x => x.PostedDate.Month == condition.Month);
			}

			if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
			{
				posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
			}

			return posts;
		}

		public async Task<IPagedList<Post>> GetPagedPostsAsync(
		PostQuery condition,
		int pageNumber = 1,
		int pageSize = 10,
		CancellationToken cancellationToken = default)
		{
			return await FilterPosts(condition).ToPagedListAsync(
				pageNumber, pageSize,
				nameof(Post.PostedDate), "DESC", cancellationToken);
		}

		public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
		PostQuery condition,
		Func<IQueryable<Post>, IQueryable<T>> mapper,
		int pageNumber = 1,
		int pageSize = 10)
		{
			var posts = FilterPosts(condition);
			var projectedPosts = mapper(posts);

			return await projectedPosts.ToPagedListAsync(
				pageNumber, pageSize,
				nameof(Post.PostedDate), "DESC");
		}
	}
}
