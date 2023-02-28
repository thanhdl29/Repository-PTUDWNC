using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;



namespace TatBlog.WinApp
{
	internal class Program
	{
		static void Main(string[] args)
		{	// Tạo đối tuọngh DbContext để quản lý phiên làm việc
			// Với CSDL và trạng thái của các đối tuọng 
			var context = new BlogDbContext();

			var posts = context.Posts
				.Where(p => p.Published)
				.OrderBy(p => p.Title)
				.Select(p => new
				{
					Id = p.Id,
					Title = p.Title,
					ViewCount = p.ViewCount,
					PostedDate = p.PostedDate,
					Author = p.Author.FullName,
					Category = p.Category.Name,
				}) .ToList();
				
			/*// Tạo đối tượng khởi tạo dữ liệu mẫu
			var seeder = new DataSeeder(context);
			// Gọi hàm Initialize để nhập dữ liệu mẫu 
			seeder.Initialize();
			// Dọc danh sách tác giả từ CSDL
			var authors = context.Authors.ToList();
			// Xuất danh sách tác giả ra màn hình
			Console.WriteLine("{0,4}{1,-30}{2,-30}{3,12}", "ID", "Full Name", "Email", "Joined Date");*/
			foreach (var post in posts) 
			{
				Console.WriteLine("ID			: {0}", post.Id);
				Console.WriteLine("Title		: {0}", post.Title);
				Console.WriteLine("View			: {0}", post.ViewCount);
				Console.WriteLine("Date			: {0:MM/dd/yyyy}", post.PostedDate);
				Console.WriteLine("Author		: {0}", post.Author);
				Console.WriteLine("Category		: {0}", post.Category);
				Console.WriteLine("".PadRight(80, '-'));
				
			}

			
		}
		
	}

}