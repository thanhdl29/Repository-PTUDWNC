using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
	public interface IPostRepository
	{
		Task<Post> GetPostBySlugAsync(
			string slug, CancellationToken cancellationToken = default);
		Task<Post> GetCachedPostBySlugAsync(
			string slug, CancellationToken cancellationToken = default);
		Task<Post> GetPostByIdAsync(int postId);
		Task<Post> GetCachedPostByIdAsync(int postId);
		Task<IList<PostItem>> GetPostAsync(
			CancellationToken cancellationToken = default);
		Task<IPagedList<PostItem>> GetPagedPostAsync(
			IPagingParams pagingParams,
			string title = null,
			CancellationToken cancellationToken = default);
		Task<IPagedList<T>> GetPagedPostAsync<T>(
			Func<IQueryable<Post>, IQueryable<T>> mapper,
			IPagingParams pagingParams,
			string title = null,
			CancellationToken cancellationToken = default);
		Task<bool> AddOrUpdateAsync(
			Post post, CancellationToken cancellationToken = default);
		Task<bool> DeletePostAsync(
			int postId, CancellationToken cancellationToken = default);
		Task<bool> IsPostSlugExistedAsync(
			int postId,
			string slug,
			CancellationToken cancellationToken = default);

		Task<IPagedList<Post>> GetPagedPostsAsync(
		PostQuery condition,
		int pageNumber = 1,
		int pageSize = 10,
		CancellationToken cancellationToken = default);

		Task<IPagedList<T>> GetPagedPostsAsync<T>(
		PostQuery condition,
		Func<IQueryable<Post>, IQueryable<T>> mapper,
		int pageNumber = 1,
		int pageSize = 10);
	}
}
