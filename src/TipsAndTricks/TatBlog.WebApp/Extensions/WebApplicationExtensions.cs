using Microsoft.EntityFrameworkCore;
using NLog.Web;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Middlewares;

namespace TatBlog.WebApp.Extensions
{
	static class WebApplicationExtensions
	{
		public static WebApplicationBuilder ConfigureMvc(
		this WebApplicationBuilder builder)
		{
			builder.Services.AddControllersWithViews();
			builder.Services.AddResponseCompression();
			
			
			return builder;
		}
		public static WebApplicationBuilder ConfigureNlog(
            this WebApplicationBuilder builder)
		{
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
			return builder;
        }
        //Đăng ký các dịch vụ với DI Container
        public static WebApplicationBuilder ConfigureServices(
			this WebApplicationBuilder buider)
		{
			buider.Services.AddDbContext<BlogDbContext>(options =>
				options.UseSqlServer(
					buider.Configuration
						.GetConnectionString("DefaultConnection")));
			buider.Services.AddScoped<IBlogRepository, BlogRepository>();
			buider.Services.AddScoped<IDataSeeder, DataSeeder>();
			buider.Services.AddScoped<IMediaManager, LocalFileSystemMediaManager>();
			return buider;
		}
		public static WebApplication UseRequestPipeline(
			this WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Blog/Error");
				app.UseHsts();
			}
			app.UseResponseCompression();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			return app;
		}
		public static IApplicationBuilder UseDataSeeder(
			this IApplicationBuilder app)
		{
			using var scope = app.ApplicationServices.CreateScope();
			try
			{
				scope.ServiceProvider
					.GetRequiredService<IDataSeeder>()
					.Initialize();
			}
			catch(Exception ex)
			{
				scope.ServiceProvider
					.GetRequiredService<ILogger<Program>>()
					.LogError(ex , "Could not insert data into database");
			}
			app.UseRouting();
			app.UseMiddleware<UserActivityMiddleware>();
			return app;
		}
		
		

	}
	
}
