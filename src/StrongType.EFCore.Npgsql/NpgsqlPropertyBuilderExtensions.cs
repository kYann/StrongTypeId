using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.EFCore.Npgsql
{
	public static class NpgsqlPropertyBuilderExtensions
	{
		public static IConventionPropertyBuilder HasValueGenerationStrategy(this IConventionPropertyBuilder propertyBuilder,
			NpgsqlValueGenerationStrategy? valueGenerationStrategy, bool fromDataAnnotation = false)
		{
			if (propertyBuilder.CanSetAnnotation(NpgsqlAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy,
				fromDataAnnotation))
			{
				propertyBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);

				if (valueGenerationStrategy != NpgsqlValueGenerationStrategy.SequenceHiLo)
				{
					propertyBuilder.HasHiLoSequence(null, null, fromDataAnnotation);
				}

				return propertyBuilder;
			}

			return null;
		}

		public static NpgsqlValueGenerationStrategy? SetValueGenerationStrategy(this IConventionProperty property,
			NpgsqlValueGenerationStrategy? value, bool fromDataAnnotation = false)
		{
			////CheckValueGenerationStrategy(property, value);

			property.SetOrRemoveAnnotation(NpgsqlAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

			return value;
		}

		public static PropertyBuilder UseIdentityColumnWorkaround(this PropertyBuilder propertyBuilder)
		{
			IMutableProperty property = propertyBuilder.Metadata;

			////property.SetValueGenerationStrategy(NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
			property.SetOrRemoveAnnotation(NpgsqlAnnotationNames.ValueGenerationStrategy,
				NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
			property.SetHiLoSequenceName(null);
			property.SetHiLoSequenceSchema(null);

			return propertyBuilder;
		}
	}
}
