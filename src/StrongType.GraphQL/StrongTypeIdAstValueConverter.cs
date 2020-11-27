using GraphQL.Language.AST;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.GraphQL
{
	public class StrongTypeIdAstValueConverter<TType> : IAstFromValueConverter where TType : IStrongTypeId
	{
		public IValue Convert(object value, IGraphType type)
		{
			return new StrongTypeIdValue<TType>((TType)value);
		}

		public bool Matches(object value, IGraphType type)
		{
			return value is IStrongTypeId;
		}
	}
}
