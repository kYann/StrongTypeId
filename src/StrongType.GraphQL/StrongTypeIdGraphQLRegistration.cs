using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.GraphQL
{
	public static class StrongTypeIdGraphQLRegistration
	{
		public static ISchema AddStrongTypeId<TType>(this ISchema schema) where TType : IStrongTypeId
		{
			GraphTypeTypeRegistry.Register(typeof(TType), typeof(StrongTypeIdGraphType<TType>));
			schema.RegisterValueConverter(new StrongTypeIdAstValueConverter<TType>());

			return schema;
		}
	}
}
