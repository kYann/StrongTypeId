using GraphQL.Language.AST;
using GraphQL.Types;
using System;

namespace StrongType.GraphQL
{
	public class StrongTypeIdGraphType<TType> : ScalarGraphType where TType : IStrongTypeId
	{
		public StrongTypeIdGraphType()
		{
			Name = typeof(TType).Name;
		}

		public override object ParseLiteral(IValue value)
		{
			if (value is StrongTypeIdValue<TType> strongTypeIdValue)
				return ParseValue(strongTypeIdValue.Value);

			return value is StringValue stringValue
			   ? ParseValue(stringValue.Value)
			   : null;
		}

		public override object ParseValue(object value)
		{
			if (value is TType)
				return value;
			var typeConverter = new StrongTypeIdConverter(typeof(TType));
			return (TType)typeConverter.ConvertFrom(value);
		}

		public override object Serialize(object value)
		{
			if (value is IStrongTypeId strongTypeValue)
				return strongTypeValue.GetValue();
			return ParseValue(value);
		}
	}
}
