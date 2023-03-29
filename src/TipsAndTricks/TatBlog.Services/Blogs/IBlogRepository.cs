﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
	public interface IBlogRepository
	{
		//Tìm bài viết có định danh là slug và được đăng vào Day, month, year
		Task<Post> GetPostAsync(
			int year,
			int mounth,
			int day,
			string slug,
			CancellationToken cancellationToken = default);
		Task<IList<Post>> GetPopularArticlesAsync(
			int numPosts,
			CancellationToken cancellationToken = default);
		Task<IList<CategoryItem>> GetCategoriesAsync(
				bool showOnMenu = false,
				CancellationToken cancellationToken = default);


		Task<bool> IsPostSlugExistedAsync(
			int postId, string slug,
			CancellationToken cancellationToken = default);
		Task IncreaseViewCountAsync(
			int postId,
			CancellationToken cancellationToken = default);
		Task<IPagedList<TagItem>> GetPagedTagsAsync(
			IPagingParams pagingParams,
			CancellationToken cancellationToken = default);
		Task<Tag> FundTagBySlugAsync(string slug, CancellationToken cancellationToken = default);
		Task<IList<TagItem>> GetAllTagAsync(CancellationToken cancellationToken = default);
		Task DeleteTag(int id,CancellationToken cancellationToken = default);
		//Tìm một chuyên mục (Category) theo tên định danh (slug). 
		Task<Category> FundCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);
		//Tìm một chuyên mục theo mã số cho trước
		Task<CategoryItem> FundCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
		//Thêm hoặc cập nhật một chuyên mục/chủ đề. 
		Task AddCategory(string name, string urlslug, string description, CancellationToken cancellationToken = default);
		//Xóa một chuyên mục theo mã số cho trước. 
		Task DeleteCategory(int id, CancellationToken cancellationToken = default);
		//Kiểm tra tên định danh (slug) của một chuyên mục đã tồn tại hay chưa
		Task<bool> CheckSlug(string slug, CancellationToken cancellationToken = default);
		//Lấy và phân trang danh sách chuyên mục, kết quả trả về kiểu IPagedList<CategoryItem>
		Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);
		//Tìm và phân trang các bài viết thỏa mãn điều kiện tìm kiếm được cho trong 
		//đối tượng PostQuery(kết quả trả về kiểu IPagedList<Post>)

		/*Task<IList<Post>> FindPostByPostQueryAsync(PostQuery postQuery, IPagingParams pagingParams,
		CancellationToken cancellationToken = default);*/

		Task<IPagedList<Post>> GetPagedPostsAsync(
			PostQuery condition,
			int pageNumber = 1,
			int pageSize = 10,
			CancellationToken cancellationToken = default);
		/*Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default);*/

		//Task<IPagedList<Post>> GetPagedPostsAsync(PostQuery condition,int pageNumber = 1,int pageSize = 10, CancellationToken cancellationToken = default);
		/*Task<IPagedList<T>> GetPagedPostsAsync<T>(
		PostQuery condition,
		IPagingParams pagingParams,
		Func<IQueryable<Post>, IQueryable<T>> mapper);*/
		Task<Post> GetPostByIdAsync(
		int postId, bool includeDetails = false,
		CancellationToken cancellationToken = default);
		//2.1 lab02
		Task<IPagedList<Post>> GetPostByQuery(PostQuery query,
			int pageNimber = 1,
			int pageSize = 10,
			CancellationToken cancellationToken = default);
		//
		Task<Post> CreateOrUpdatePostAsync(
		Post post, IEnumerable<string> tags,
		CancellationToken cancellationToken = default);
		// Tìm 1 thẻ tag theo tên định danh slug
		Task<Tag> GetTagAsync(
		string slug, CancellationToken cancellationToken = default);
		Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default);
		Task<Post> GetPostBySlugAsync(
			string slug, bool published = false,
			CancellationToken cancellationToken = default);



		Task DeletePost(int id, CancellationToken cancellation = default);
		Task UpdatePostAsync(int id, CancellationToken cancellationToken = default);
	}
}
