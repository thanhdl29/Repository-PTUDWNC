using FluentValidation;
using System.Reflection;

namespace TatBlog.WebApi.Validation
{
	public static class FluentValidationDependencyInjection
	{
		public static WebApplicationBuilder ConfigureFluentValidation(
			this WebApplicationBuilder builder)
		{
			// Scan and register all validations in given assembbly
			builder.Services.AddValidatorsFromAssembly(
				Assembly.GetExecutingAssembly());
			return builder;
		}
	}
}
