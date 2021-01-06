using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace StrongType.EFCore
{
	public class StrongTypeIdConversionConvention : IPropertyAddedConvention,
		IEntityTypeAddedConvention
	{
		private ValueConverter CreateValueConverter(Type strongTypeIdType, Type idType)
		{
			var converterType = typeof(StrongTypeIdValueConverter<,>).MakeGenericType(strongTypeIdType, idType);

			return (ValueConverter)Activator.CreateInstance(converterType);
		}

		public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
		{
			if (propertyBuilder.Metadata.ClrType is null)
				return;
			Type idType;
			var isStrongTypeId = StrongTypeIdHelper.IsStrongTypeId(propertyBuilder.Metadata.ClrType, out idType);
			if (!isStrongTypeId)
				return;

			var valueConverter = CreateValueConverter(propertyBuilder.Metadata.ClrType, idType);
			propertyBuilder.HasConversion(valueConverter);
		}

		public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			if (entityTypeBuilder.Metadata.ClrType is null)
				return;

			var properties = entityTypeBuilder.Metadata.ClrType.GetProperties()
				.Where(p => StrongTypeIdHelper.IsStrongTypeId(p.PropertyType))
				.ToList();

			if (entityTypeBuilder.Metadata is IMutableEntityType entityType)
			{
				foreach (var property in properties)
				{
					entityType.AddProperty(property.Name, property.PropertyType, property);
				}
			}
		}
	}
}
