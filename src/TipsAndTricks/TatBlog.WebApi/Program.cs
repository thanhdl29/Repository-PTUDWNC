using TatBlog.WebApi.Endpoints;
using TatBlog.WebApi.Extension;
using TatBlog.WebApi.Mapster;
using TatBlog.WebApi.Validation;

var builder = WebApplication.CreateBuilder(args);
{
	// add services to the container
	builder
		.ConfigureCors()
		.ConfigureNlog()
		.ConfigureServices()
		.ConfigureSwaggerOpenApi()
		.ConfigureFluentValidation()
		.ConfigureMapster();
}
var app = builder.Build();
{
	app.SetupRequesPipeline();
	app.MapAuthorEndpoints();
	app.MapCategoryEndpoints();
	app.MapPostEndpoints();
	app.Run();
}

//app.Run();

