using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings
{
	internal class AuthorMap : IEntityTypeConfiguration<Author>
	{
		public void Configure(EntityTypeBuilder<Author> biulder)
		{
			biulder.ToTable("Authors");
			biulder.HasKey(a => a.Id);
			biulder.Property(a => a.FullName)
				.IsRequired()
				.HasMaxLength(100);
			biulder.Property(a => a.UrlSlug)
				.HasMaxLength(100)
				.IsRequired();
			biulder.Property(a => a.ImageUrl)
				.HasMaxLength(500);
			biulder.Property(a => a.Email)
				.HasMaxLength(150);
			biulder.Property(a => a.JoinedDate)
				.HasColumnType("datetime");
			biulder.Property(a => a.Notes)
				.HasMaxLength(500);

		}
	}
}
