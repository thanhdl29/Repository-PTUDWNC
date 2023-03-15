using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
	public class PostValidator	: AbstractValidator<PostEditModel>
	{
		private readonly IBlogRepository _blogRepository;
		public PostValidator(IBlogRepository blogRepository)
		{
			_blogRepository = blogRepository;

			RuleFor(x => x.Title)
				.NotEmpty()
				.MaximumLength(500);
			RuleFor(x => x.ShortDescription)
				.NotEmpty();
			RuleFor(x => x.Description)
				.NotEmpty();
			RuleFor(x => x.Meta)
				.NotEmpty()
				.MaximumLength(1000);
			RuleFor(x => x.UrlSlug)
				.NotEmpty()
				.MaximumLength(1000);
			RuleFor(x => x.UrlSlug)
				.MustAsync(async (postModel, slug, cancellationToken) =>
					!await blogRepository.IsPostSlugExistedAsync(
						postModel.Id, slug, cancellationToken))
				.WithMessage("Slug '{PropertyValue}' đã được sử dụng");

		}
	}
}
