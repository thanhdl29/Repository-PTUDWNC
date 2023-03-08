using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
{
	//Thêm các dịc vụ yêu cầu bởi MVC Framework
	builder.Services.AddControllersWithViews();
}
var app = builder.Build();
{	
	// Cấu hình HTTP Request pipeline 
	// Thêm middleware để hiển thị thông báo lỗi 
	if (app.Environment.IsDevelopment()) 
	{
		app.UseDeveloperExceptionPage();
	}
	else 
	{ 
		app.UseExceptionHandler("/Blog/Error");
		// Thêm middleware cho việc áp dụng HSTS (thêm header
		// Strict-Transport-Security vào HTTP Response).
		app.UseHsts();
		
	}
	//Thêm middleware để chuyển hướng HTTP sang HTTPS
	app.UseHttpsRedirection();
	// Thêm middleware phục vụ các yêu cầu liên quan
	// tới các nội dung tập tin tĩnh như hình ảnh, css,...
	app.UseStaticFiles();
	// Thêm middlaware lựa chọn endpoint phù hợp nhất 
	// để xử lý 1 HTTP requesr
	app.UseRouting();
	//Định nghĩa routetemplate, route cóntraint cho các 
	// endpoints kết hợp với các action trong controller
	app.MapControllerRoute(
		name: "default",
		pattern: "{controller = Blog}/{action=Index}/{id?}");

} 

app.Run();
