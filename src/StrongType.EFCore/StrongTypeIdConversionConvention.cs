using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace StrongType.EFCore
{
	public class StrongTypeIdConversionConvention : IPropertyAddedConvention,
		IEntityTypeAddedConvention,
		IEntityTypeBaseTypeChangedConvention
	{
		private ValueConverter CreateValueConverter(Type strongTypeIdType, Type idType)
		{
			var converterType = typeof(StrongTypeIdValueConverter<,>).MakeGenericType(strongTypeIdType, idType);

			return (ValueConverter)Activator.CreateInstance(converterType);
		}

		private ValueComparer CreateValueComparer(Type strongTypeIdType, Type idType)
		{
			var converterType = typeof(StrongTypeIdValueComparer<,>).MakeGenericType(strongTypeIdType, idType);

			return (ValueComparer)Activator.CreateInstance(converterType);
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
			var valueComparer = CreateValueComparer(propertyBuilder.Metadata.ClrType, idType);
			propertyBuilder.HasConversion(valueConverter)
				.HasValueComparer(valueComparer);
		}

		private void Process(IConventionEntityTypeBuilder entityTypeBuilder)
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
					entityTypeBuilder.Property(property);
					//entityType.AddProperty(property.Name, property.PropertyType, property);
				}
			}
		}

		public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			Process(entityTypeBuilder);
		}

        public void ProcessEntityTypeBaseTypeChanged(IConventionEntityTypeBuilder entityTypeBuilder, IConventionEntityType newBaseType, IConventionEntityType oldBaseType, IConventionContext<IConventionEntityType> context)
        {
			if (newBaseType == oldBaseType)
				return;
			Process(entityTypeBuilder);
		}
    }
}
