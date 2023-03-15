using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class PostsController : Controller
	{
	
		private readonly IBlogRepository _blogRepository;
		private readonly IMapper _mapper;
		private readonly IMediaManager _mediaManager;
		public PostsController(IBlogRepository blogRepository, 
			IMapper mapper,
			IMediaManager mediaManager)
		{
			_blogRepository = blogRepository;
			_mapper = mapper;
			_mediaManager = mediaManager;
		}

		public async Task PoulatePostFilterModelAsync(PostFilterModel model)
		{
			var authors = await _blogRepository.GetAuthorsAsync();
			var categories = await _blogRepository.GetCategoriesAsync();

			model.AuthorList = authors.Select(a => new SelectListItem()
			{
				Text = a.FullName,
				Value = a.Id.ToString()
			});
			model.CategoryList = categories.Select(c => new SelectListItem()
			{
				Text = c.Name,
				Value = c.Id.ToString()
			});

		}
		public async Task<IActionResult> Index(PostFilterModel model) 
		{
			/*var postQuery = new PostQuery()
			{
				Keyword = model.Keyword,
				CategoryId = model.CategoryId,
				AuthorId = model.AuthorId,
				Year = model.Year,
				Month = model.Month
			};*/
			var postQuery = _mapper.Map<PostQuery>(model);
			ViewBag.PostsList = await _blogRepository
				.GetPagedPostsAsync(postQuery, 1, 10);

			await PoulatePostFilterModelAsync(model);
			return View(model);
		}

		public async Task PopulatePostEditModelAsync(PostEditModel model)
		{
			var authors = await _blogRepository.GetAuthorsAsync();
			var categories = await _blogRepository.GetCategoriesAsync();

			model.AuthorList = authors.Select(a => new SelectListItem()
			{
				Text = a.FullName,
				Value = a.Id.ToString()
			});
			model.CategoryList = categories.Select(c => new SelectListItem()
			{
				Text = c.Name,
				Value = c.Id.ToString()
			});

		}

		public async Task<IActionResult> Edit(int id = 0)
		{
			//Id = 0 <=> Thêm bài viết mới
			//ID >0 : Đọc dữ liệu ccuar bài viết từ CSDL
			var post = id >0
				?await _blogRepository.GetPostByIdAsync(id, true) : null;
			//Tạo view model từ dữ liệu của bài viết
			var model = post == null
				? new PostEditModel()
				: _mapper.Map<PostEditModel>(post);
			//Gán các giá trị khác cho view model
			await PopulatePostEditModelAsync(model);
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(PostEditModel model)
		{
			if (!ModelState.IsValid)
			{
				await PopulatePostEditModelAsync(model);
				return View(model);
			}
			var post = model.Id >0
				? await _blogRepository.GetPostByIdAsync(model.Id)
				: null;

			if (post == null)
			{
				post = _mapper.Map<Post>(model);

				post.Id = 0;
				post.PostedDate = DateTime.Now;
			}

			else
			{
				_mapper.Map(model, post);
				post.Category = null;
				post.ModifiedDate = DateTime.Now;
			}
			//Nếu người dùng có upload hình ảnh minh hoạ cho bài viết
			if (model.ImageFile?.Length>0)
			{
				//Thì thực hiện việc lưu tập tin vào thư mục upload

				var newImagePath = await _mediaManager.SaveFileAsync(
					model.ImageFile.OpenReadStream(),
					model.ImageFile.FileName,
					model.ImageFile.ContentType);

				//Nếu lưu thành công, xoá tập tin hình ảnh cũ nếu có
				if(!string.IsNullOrWhiteSpace(newImagePath))
				{
					await _mediaManager.DeleteFileAsync(post.ImageUrl);
					post.ImageUrl = newImagePath;

				}
			}
			await _blogRepository.CreateOrUpdatePostAsync(
				post, model.GetSelectedTags());
			return RedirectToAction(nameof(Index));

		}
		[HttpPost]
		public async Task<IActionResult> VerifyPostSlug(
			int id, string urlSlug)
		{
			var slugExisted = await _blogRepository
				.IsPostSlugExistedAsync(id, urlSlug);
			return slugExisted
				? Json($"Slug '{urlSlug}' đã được sủ dụng")
				: Json(true);
		}

	}
}
