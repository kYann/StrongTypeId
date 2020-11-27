using GraphQL.Language.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.GraphQL
{
	public class StrongTypeIdValue<TType> : ValueNode<TType> where TType : IStrongTypeId
	{
		public StrongTypeIdValue(TType value)
		{
			Value = value;
		}

		protected override bool Equals(ValueNode<TType> node)
		{
			return Value.Equals(node.Value);
		}
	}
}
