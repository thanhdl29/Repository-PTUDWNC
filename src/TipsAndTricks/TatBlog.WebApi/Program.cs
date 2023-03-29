using TatBlog.WebApi.Extension;

var builder = WebApplication.CreateBuilder(args);
{
	// add services to the container
	builder
		.ConfigureCors()
		.ConfigureNlog()
		.ConfigureServices()
		.ConfigureSwaggerOpenApi();
}
var app = builder.Build();
{
	app.SetupRequesPipeline();
	app.Run();
}

//app.Run();

