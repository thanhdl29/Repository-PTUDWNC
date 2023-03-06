using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using System;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

// Tạo đối tuọngh DbContext để quản lý phiên làm việc
// Với CSDL và trạng thái của các đối tuọng 
var context = new BlogDbContext();

IBlogRepository blogRepo = new BlogRepository(context);

/*var pagingParams = new PagingParams
{
	PageNumber = 1,
	PageSize = 5,
	SortColumn = "name",
	SortOrder = "DESC"
};
var tagsList = await blogRepo.GetPagedTagsAsync(pagingParams);
Console.WriteLine("{0,5}{1,-50}{2,10}", "ID", "Name", "Count");
foreach (var item in tagsList)
{
	Console.WriteLine("{0,5}{1,-50}{2,10}",
		 item.Id, item.Name, item.PostCount);

}*/

//var posts = await blogRepo.GetPopularArticlesAsync(3);
/*var categories = await blogRepo.GetCategoriesAsync();
Console.WriteLine("{0,5}{1,-50}{2,10}",
	"ID", "Name", "Count");
foreach (var item in categories)
{
	 Console.WriteLine("{0,5}{1,-50}{2,10}",
		 item.Id, item.Name, item.PostCount);
}*/

/*var posts = context.Posts
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
	})
	.ToList();*/

// Tạo đối tượng khởi tạo dữ liệu mẫu
//var seeder = new DataSeeder(context);
// Gọi hàm Initialize để nhập dữ liệu mẫu 
//seeder.Initialize();
// Dọc danh sách tác giả từ CSDL
//var authors = context.Authors.ToList();
// Xuất danh sách tác giả ra màn hình
//Console.WriteLine("{0,4}{1,-30}{2,-30}{3,12}", "ID", "Full Name", "Email", "Joined Date");
/*foreach (var post in posts)
{
	//Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12:MM/dd/yyyy}", author.Id, author.FullName, author.Email, author.JoinedDate);
	Console.WriteLine("ID           : {0}", post.Id);
	Console.WriteLine("Title        : {0}", post.Title);
	Console.WriteLine("View         : {0}", post.ViewCount);
	Console.WriteLine("Date         : {0:MM/dd/yyyy}", post.PostedDate);
	Console.WriteLine("Author       : {0}", post.Author);
	Console.WriteLine("Category     : {0}", post.Category);
	Console.WriteLine("".PadRight(80, '-'));

}*/

// Cau 1:a) Tìm một thẻ (Tag) theo tên định danh (slug)
/*string slug = "Google";
var tag = await blogRepo.FundTagBySlugAsync(slug);
Console.WriteLine("{0,-5}{1,-20}{2,-30}", "ID", "Name", "Description");
Console.WriteLine("{0,-5}{1,-20}{2,-30}", tag.Id, tag.Name, tag.Description);*/
//Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó. Kết quả trả về kiểu IList<TagItem>.
/*var tags = await blogRepo.GetAllTagAsync();
Console.WriteLine("{0,5}{1,-50}{2,10}",
	"ID", "Name", "Count");
foreach(var item in tags)
{
	Console.WriteLine("{0,5}{1,-50}{2,10}", item.Id, item.Name, item.PostCount);
}*/
//Xoá 1 thẻ theo mã cho trước
/*Console.WriteLine("Nhap ma the can xoa: ");
int nhap = Convert.ToInt32(Console.ReadLine());
var TagDelete = blogRepo.DeleteTag(nhap);
var tags = context.Tags
	.OrderBy(x => x.Name)
	.Select(x => new
	{
		Id = x.Id,
		Name = x.Name,
		UrlSlug = x.UrlSlug,
		Description = x.Description,
		PostCount = x.Posts.Count()
	}).ToList();
if(tags.Count > 0)
{
	Console.WriteLine("Danh sách thẻ còn lại: ");
	foreach(var tag in tags)
	{
		Console.WriteLine("ID           : {0}", tag.Id);
		Console.WriteLine("Name        : {0}", tag.Name);
		Console.WriteLine("UrlSlug         : {0}", tag.UrlSlug);
		Console.WriteLine("Description        : {0}",tag.Description);
		Console.WriteLine("PostCount       : {0}",tag.PostCount);
		Console.WriteLine("".PadRight(80, '-'));

	}
}
else
{
	Console.WriteLine("Không tìm thấy thẻ!");
}*/
//Tìm một chuyên mục (Category) theo tên định danh (slug). 
/*string slug = "Google";
var category = await blogRepo.FundTagBySlugAsync(slug);
Console.WriteLine("{0,-5}{1,-20}{2,-30}", "ID", "Name", "Description");
Console.WriteLine("{0,-5}{1,-20}{2,-30}", category.Id, category.Name, category.Description);*/
//Tìm một chuyên mục theo mã số cho trước
Console.WriteLine("Nhap ma so chuyen muc can tim: ");
int nhapMs = Convert.ToInt32(Console.ReadLine());
var findById = await blogRepo.FundCategoryByIdAsync(nhapMs);
Console.WriteLine("{0,-5} {1,-20} {2,-30} {3,-20} {4,-30}", "ID", "Name", "UrlSlug", "Description", "PostCount");
Console.WriteLine("{0,-5} {1,-20} {2,-30} {3,-20} {4,-30}", findById.Id, findById.Name, findById.UrlSlug, findById.Description, findById.PostCount);

