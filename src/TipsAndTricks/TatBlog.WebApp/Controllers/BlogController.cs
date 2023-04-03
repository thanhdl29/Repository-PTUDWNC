using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using SlugGenerator;
using System.Drawing.Printing;

namespace TatBlog.WebApp.Controllers
{
	public class BlogController	: Controller
	{

		private readonly IBlogRepository _blogRepository;
		
		public BlogController(IBlogRepository blogRepository)
		{
			_blogRepository = blogRepository;
			
		}

		public async Task<IActionResult> Index(
			[FromQuery(Name ="k")] string keywork = null,
			[FromQuery(Name = "p")] int pageNumber = 1,
			[FromQuery(Name = "ps")] int pageSize = 10)
		{
			var postQuery = new PostQuery()
			{
				Publishedonly = true,
				Keyword = keywork
			};
			var postsList = await _blogRepository
				.GetPagedPostsAsync(postQuery, pageNumber, pageSize);
			ViewBag.PostQuery = postQuery;
			return View(postsList);
		}
		public async Task<IActionResult> Tag(string slug)
		{
			var postQuery = new PostQuery
			{
				TagSlug = slug
			};
			var post = await _blogRepository.GetPostByQuery(postQuery);
			var tag = await _blogRepository.GetTagAsync(slug);
			ViewData["Tags"] = tag;

			return View(post);

		}
		public async Task<IActionResult> Post(int year, int month, int day, string slug)
		{
			var post = await _blogRepository.GetPostAsync(year, month, day, slug);
			await _blogRepository.IncreaseViewCountAsync(post.Id);
			return View(post);
		}
		//2.1 Lab02
		public async Task<IActionResult> Category(string slug)
		{
			var postQuery = new PostQuery
			{
				CategorySlug = slug
			};
			var post = await _blogRepository.GetPostByQuery(postQuery);
			return View(post);
		}
		//2.2 Lab02
		public async Task<IActionResult> Author(string slug)
		{
			var postQuery = new PostQuery
			{
				AuthorSlug = slug
			};
			var post = await _blogRepository.GetPostByQuery(postQuery);
			return View(post);
		}
		

	

		public IActionResult About()
			=> View();
		public IActionResult Contact()
			=> View();

		public IActionResult Rss()
			=> Content("Nội dung sẽ được cập nhật");

	}
}
