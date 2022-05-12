using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.TestUtilities;
using StrongType.EFCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace StrongType.Tests
{
	public class EFCoreTests
	{
		public record BlogId(int Id) : StrongTypeId<int>(Id);

		public class Blog
		{
			public BlogId Id { get; set; }
		}

		[Fact]
		public void WhenKeyIsStrongTypeId_ThenMappingHasConversion()
		{
			var model = BuildModel((b) =>
			{
				b.Entity<Blog>();
			});
			var blogEntityType = model.FindRuntimeEntityType(typeof(Blog));

			var property = blogEntityType.GetProperty("Id");
			Assert.NotNull(property.GetValueConverter());

			var db = InMemoryTestHelpers.Instance.CreateContext(model);

			db.Add(new Blog() { Id = new BlogId(1) });
			db.SaveChanges();
			var blog = db.Set<Blog>().FirstOrDefault();

			Assert.NotNull(blog);
		}



		#region Support

		private IModel BuildModel(Action<ModelBuilder> buildAction, CultureInfo cultureInfo = null)
		{
			var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder(configure: (conf) =>
            {
				new StrongTypeConventionSetPlugin().ModifyConventions(conf.Conventions);
            });
			buildAction(builder);
			return builder.FinalizeModel();
		}

		private Microsoft.EntityFrameworkCore.Metadata.IEntityType BuildEntityType(string entityTypeName, Action<EntityTypeBuilder> buildAction, CultureInfo cultureInfo = null)
			=> BuildModel((b) => buildAction(b.Entity(entityTypeName)), cultureInfo).GetEntityTypes().Single();

		#endregion
	}
}
