﻿using FluentValidation;
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
	public static class CategoryEndpoints
	{
		public static WebApplication MapCategoryEndpoints(
			this WebApplication app)
		{
			var routeGroupBuilder = app.MapGroup("/api/categories");
			routeGroupBuilder.MapGet("/", GetCategories)
				.WithName("GetCategoris")
				.Produces<ApiResponse<PaginationResult<CategoryItem>>>();

			routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetails)
				.WithName("GetCategoryById")
				.Produces<ApiResponse<CategoryItem>>();
				//.Produces(404);

			routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9_-]+$)}/posts",
				GetPostsByCategorySlug)
				.WithName("GetCategoryBySlug")
				.Produces<ApiResponse<PaginationResult<PostDto>>>();

			routeGroupBuilder.MapPost("/", AddCategory)
				.WithName("addnewcategory")
				.AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
				.Produces(401)
				.Produces<ApiResponse<CategoryItem>>();

			//routeGroupBuilder.MapPost("/{id:int}/avatar", SetCAtegoryPicture)
			//	.WithName("SetAuthorPicture")
			//	.Accepts<IFormFile>("multipart/form-data")
			//	.RequireAuthorization()
			//	//.Produces<string>()
			//	.Produces(401);
			routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
				.WithName("UpdateCategory")
				.AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
				//Produces(204)
				//.RequireAuthorization()
				.Produces(401)
				.Produces<ApiResponse<string>>();
			//.Produces(409);
			routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
				.WithName("DeleteCategory")
				.Produces(204)
				.Produces(404);

			return app;
		}

		private static async Task<IResult> GetCategories(
			[AsParameters] CategoryFilterModel model,
			ICategoryRepository categoryRepository)
		{
			var categoryList = await categoryRepository
				.GetPagedCategorysAsync(model, model.Name);
			var paginationResult =
				new PaginationResult<CategoryItem>(categoryList);
			return Results.Ok(ApiResponse.Success(paginationResult));
		}
		private static async Task<IResult> GetCategoryDetails(
			int id,
			ICategoryRepository categoryRepository,
			IMapper mapper)
		{
			var category = await categoryRepository.GetCachedCategoryByIdAsync(id);
			return category == null
				? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
					$"Không tìm thâys chuyên mục có mã số {id}"))
				: Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));
		}

		private static async Task<IResult> GetPostsByCategoryId(
			int id,
			[AsParameters] PagingModel pagingModel,
			IBlogRepository blogRepository)
		{
			var postQuery = new PostQuery()
			{
				CategoryId = id,
				Publishedonly = true
			};
			var postList = await blogRepository.GetPagedPostsAsync(
				postQuery, pagingModel,
				posts => posts.ProjectToType<PostDto>());

			var paginationResult = new PaginationResult<PostDto>(postList);
			return Results.Ok(ApiResponse.Success(paginationResult));
		}
		private static async Task<IResult> GetPostsByCategorySlug(
			[FromRoute] string slug,
			[AsParameters] PagingModel pagingModel,
			IBlogRepository blogRepository)
		{
			var postQuery = new PostQuery()
			{
				categorySlug = slug,
				Publishedonly = true
			};
			var postList = await blogRepository.GetPagedPostsAsync(
				postQuery, pagingModel,
				posts => posts.ProjectToType<PostDto>());

			var paginationResult = new PaginationResult<PostDto>(postList);
			return Results.Ok(ApiResponse.Success(paginationResult));
		}

		private static async Task<IResult> AddCategory(
			CategoryEditModel model,
			ICategoryRepository categoryRepository,
			IMapper mapper)
		{

			if (await categoryRepository
				.IsCategorySlugExistedAsync(0, model.UrlSlug))
			{
				return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
					$"Slug '{model.UrlSlug}' đã được sử dụng"));

			}

			var category = mapper.Map<Category>(model);
			await categoryRepository.AddOrUpdateAsync(category);

			return Results.Ok(ApiResponse.Success(
				mapper.Map<CategoryItem>(category), HttpStatusCode.Created));
		}
		//private static async Task<IResult> SetCategoryPicture(
		//	int id, IFormFile imageFile,
		//	IAuthorRepository authorRepository,
		//	IMediaManager mediaManager)
		//{
		//	var imageUrl = await mediaManager.SaveFileAsync(
		//		imageFile.OpenReadStream(),
		//		imageFile.FileName, imageFile.ContentType);

		//	if (string.IsNullOrWhiteSpace(imageUrl))
		//	{
		//		return Results.Ok(ApiResponse.Fail(
		//			HttpStatusCode.BadRequest, "Không lưu được tập tin"));
		//	}
		//	await authorRepository.SetImageUrlAsync(id, imageUrl);
		//	return Results.Ok(ApiResponse.Success(imageUrl));
		//}
		private static async Task<IResult> UpdateCategory(
			int id,
			IValidator<CategoryEditModel> validator,
			CategoryEditModel model,
			ICategoryRepository categoryRepository,
			IMapper mapper)
		{
			var validationResult = await validator.ValidateAsync(model);
			if (!validationResult.IsValid)
			{
				return Results.Ok(ApiResponse.Fail(
					HttpStatusCode.BadRequest, validationResult));
			}

			if (await categoryRepository
				.IsCategorySlugExistedAsync(id, model.UrlSlug))
			{
				return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
					$"Slug '{model.UrlSlug}' đã được dử dụng"));
			}
			var category = mapper.Map<Category>(model);
			category.Id = id;

			return await categoryRepository.AddOrUpdateAsync(category)
				? Results.Ok(ApiResponse.Success("Category is updated",
				HttpStatusCode.NoContent))
				: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
				"Could not find category"));
		}
		private static async Task<IResult> DeleteCategory(
			int id, ICategoryRepository categoryRepository)
		{
			return await categoryRepository.DeleteCategoryAsync(id)
				? Results.Ok(ApiResponse.Success("Category is updated",
				HttpStatusCode.NoContent))
				: Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
				"Could not find category")); ;
		}
	}
}
