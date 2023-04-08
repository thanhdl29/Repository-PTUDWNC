using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
	public static class PostEndpoints
	{
		public static WebApplication MapPostEndpoints(
			this WebApplication app)
		{
			var routeGroupBuilder = app.MapGroup("/api/post");
			routeGroupBuilder.MapGet("/", GetPost)
				.WithName("GetPost")
				.Produces<ApiResponse<PaginationResult<PostItem>>>();

			routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
				.WithName("GetPostById")
				.Produces<ApiResponse<PostItem>>();
			//.Produces(404);

			routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9_-]+$)}/posts",
				GetPostsByPostSlug)
				.WithName("GetPostBySlug")
				.Produces<ApiResponse<PaginationResult<PostDto>>>();

			routeGroupBuilder.MapPost("/", AddPost)
				.WithName("AddNewPost")
				.AddEndpointFilter<ValidatorFilter<PostEditModel>>()
				.Produces(401)
				//.RequirePostization()
				.Produces<ApiResponse<PostItem>>();

			routeGroupBuilder.MapPost("/{id:int}/avatar", SetPostPicture)
				.WithName("SetPostPicture")
				.Accepts<IFormFile>("multipart/form-data")
				//.RequirePostization()
				//.Produces<string>()
				.Produces(401);
			routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
				.WithName("UpdateAnPost")
				.AddEndpointFilter<ValidatorFilter<PostEditModel>>()
				//Produces(204)
				//.RequirePostization()
				.Produces(401)
				.Produces<ApiResponse<string>>();
			//.Produces(409);
			routeGroupBuilder.MapDelete("/{id:int}", DeletePost)
				.WithName("DeleteAnPost")
				.Produces(204)
				.Produces(404);

			return app;
		}

		private static async Task<IResult> GetPost(
			[AsParameters] PostQuery model,
			IPostRepository postRepository)
		{
			var postList = await postRepository
				.GetPagedPostsAsync(model);
			var paginationResult =
				new PaginationResult<Post>(postList);
			return Results.Ok(ApiResponse.Success(paginationResult));
		}
		private static async Task<IResult> GetPostDetails(
			int id,
			IPostRepository postRepository,
			IMapper mapper)
		{
			var post = await postRepository.GetCachedPostByIdAsync(id);
			return post == null
				? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
					$"Không tìm thâys tác giả có mã số {id}"))
				: Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post)));
		}

		private static async Task<IResult> GetPostsByPostId(
			int id,
			[AsParameters] PagingModel pagingModel,
			IBlogRepository blogRepository)
		{
			var postQuery = new PostQuery()
			{
				PostId = id,
				Publishedonly = true
			};
			var postList = await blogRepository.GetPagedPostsAsync(
				postQuery, pagingModel,
				posts => posts.ProjectToType<PostDto>());

			var paginationResult = new PaginationResult<PostDto>(postList);
			return Results.Ok(ApiResponse.Success(paginationResult));
		}


		///============================================================================================Chưa sửa
		private static async Task<IResult> GetPostsByPostSlug(
			[FromRoute] string slug,
			[AsParameters] PagingModel pagingModel,
			IBlogRepository blogRepository)
		{
			var postQuery = new PostQuery()
			{
				AuthorSlug = slug,
				Publishedonly = true
			};
			var postList = await blogRepository.GetPagedPostsAsync(
				postQuery, pagingModel,
				posts => posts.ProjectToType<PostDto>());

			var paginationResult = new PaginationResult<PostDto>(postList);
			return Results.Ok(ApiResponse.Success(paginationResult));
		}

		private static async Task<IResult> AddPost(
			AuthorEditModel model,
			IAuthorRepository authorRepository,
			IMapper mapper)
		{

			if (await authorRepository
				.IsAuthorSlugExistedAsync(0, model.UrlSlug))
			{
				return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
					$"Slug '{model.UrlSlug}' đã được sử dụng"));

			}

			var auhtor = mapper.Map<Author>(model);
			await authorRepository.AddOrUpdateAsync(auhtor);

			return Results.Ok(ApiResponse.Success(
				mapper.Map<AuthorItem>(auhtor), HttpStatusCode.Created));
		}
		private static async Task<IResult> SetPostPicture(
			int id, IFormFile imageFile,
			IAuthorRepository authorRepository,
			IMediaManager mediaManager)
		{
			var imageUrl = await mediaManager.SaveFileAsync(
				imageFile.OpenReadStream(),
				imageFile.FileName, imageFile.ContentType);

			if (string.IsNullOrWhiteSpace(imageUrl))
			{
				return Results.Ok(ApiResponse.Fail(
					HttpStatusCode.BadRequest, "Không lưu được tập tin"));
			}
			await authorRepository.SetImageUrlAsync(id, imageUrl);
			return Results.Ok(ApiResponse.Success(imageUrl));
		}
		private static async Task<IResult> UpdatePost(
			int id,
			IValidator<AuthorEditModel> validator,
			AuthorEditModel model,
			IAuthorRepository authorRepository,
			IMapper mapper)
		{
			var validationResult = await validator.ValidateAsync(model);
			if (!validationResult.IsValid)
			{
				return Results.Ok(ApiResponse.Fail(
					HttpStatusCode.BadRequest, validationResult));
			}

			if (await authorRepository
				.IsAuthorSlugExistedAsync(id, model.UrlSlug))
			{
				return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
					$"Slug '{model.UrlSlug}' đã được dử dụng"));
			}
			var author = mapper.Map<Author>(model);
			author.Id = id;

			return await authorRepository.AddOrUpdateAsync(author)
				? Results.Ok(ApiResponse.Success("Author is updated",
				HttpStatusCode.NoContent))
				: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
				"Could not find author"));
		}
		private static async Task<IResult> DeletePost(
			int id, IAuthorRepository authorRepository)
		{
			return await authorRepository.DeleteAuthorAsync(id)
				? Results.Ok(ApiResponse.Success("Author is updated",
				HttpStatusCode.NoContent))
				: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
				"Could not find author")); ;
		}
	}
}
