﻿using FluentValidation;
using FluentValidation.AspNetCore;
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

		private readonly ILogger<PostsController> _logger;
		private readonly IBlogRepository _blogRepository;
		private readonly IMapper _mapper;
		private readonly IMediaManager _mediaManager;
		public PostsController(IBlogRepository blogRepository,
			IMapper mapper,
			ILogger<PostsController> logger,
			IMediaManager mediaManager)
		{
			_blogRepository = blogRepository;
			_mapper = mapper;
			_mediaManager = mediaManager;
			_logger = logger;
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
		public async Task<IActionResult> Index(PostFilterModel model,
			 int pageNumber = 1,
			 int pageSize = 10)
		{
			/*var postQuery = new PostQuery()
			{
				Keyword = model.Keyword,
				CategoryId = model.CategoryId,
				AuthorId = model.AuthorId,
				Year = model.Year,
				Month = model.Month
			};*/
			_logger.LogInformation("Tạo điều kiện truy vấn");
			var postQuery = _mapper.Map<PostQuery>(model);
			_logger.LogInformation("Lấy danh sách bài viết từ CSDL");
			ViewBag.PostsList = await _blogRepository
				.GetPagedPostsAsync(postQuery, pageNumber, pageSize);

			_logger.LogInformation("Chuẩn bị dữ liệu cho ViewModel");

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

		[HttpGet]
		public async Task<IActionResult> Edit(int id = 0)
		{
			//Id = 0 <=> Thêm bài viết mới
			//ID >0 : Đọc dữ liệu ccuar bài viết từ CSDL
			var post = id > 0
				? await _blogRepository.GetPostByIdAsync(id, true) : null;
			//Tạo view model từ dữ liệu của bài viết
			var model = post == null
				? new PostEditModel()
				: _mapper.Map<PostEditModel>(post);
			//Gán các giá trị khác cho view model
			await PopulatePostEditModelAsync(model);
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(/*PostEditModel model*/
			[FromServices] IValidator<PostEditModel> postValidator,
			PostEditModel model)
		{
			var validationResult = await postValidator.ValidateAsync(model);
			if (!validationResult.IsValid)
			{
				validationResult.AddToModelState(ModelState);
			}
			if (!ModelState.IsValid)
			{
				await PopulatePostEditModelAsync(model);
				return View(model);
			}
			var post = model.Id > 0
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
			if (model.ImageFile?.Length > 0)
			{
				//Thì thực hiện việc lưu tập tin vào thư mục upload

				var newImagePath = await _mediaManager.SaveFileAsync(
					model.ImageFile.OpenReadStream(),
					model.ImageFile.FileName,
					model.ImageFile.ContentType);

				//Nếu lưu thành công, xoá tập tin hình ảnh cũ nếu có
				if (!string.IsNullOrWhiteSpace(newImagePath))
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
		public async Task<IActionResult> ChangePublishStatus(int id)
		{
			await _blogRepository.UpdatePostAsync(id);
			
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> DeletePosts(int id)
		{
			await _blogRepository.DeletePost(id);
			return RedirectToAction(nameof(Index));
		}

		//public async Task<IActionResult>  Delete(int id)
		//{
		//	//// Tìm bài viết theo id
		//	//var post = db.Posts.Find(id);

		//	//// Xóa bài viết khỏi cơ sở dữ liệu
		//	//db.Posts.Remove(post);
		//	//db.SaveChanges();

		//	//// Trả về kết quả thành công
		//	//return Json(new { success = true });
		//	await _blogRepository.(id);

		//	return RedirectToAction(nameof(Index));
		//}
	}
}
