using HotChocolate.Language;
using HotChocolate.Types;
using System;

namespace StrongType.HotChocolate
{
	public sealed class StrongTypeIdGraphType<TValue> : ScalarType where TValue : IStrongTypeId
	{
		StrongTypeIdConverter converter = new StrongTypeIdConverter(typeof(TValue));

		public StrongTypeIdGraphType() : base(typeof(TValue).Name)
		{
		}

		// define which .NET type represents your type
		public override Type RuntimeType => typeof(TValue);

		// define which literals this type can be parsed from.
		public override bool IsInstanceOfType(IValueNode literal)
		{
			if (literal == null)
			{
				throw new ArgumentNullException(nameof(literal));
			}

			return literal is StringValueNode
				|| literal is NullValueNode;
		}

		public override object ParseLiteral(IValueNode valueSyntax, bool withDefaults = true)
		{
			if (valueSyntax == null)
			{
				throw new ArgumentNullException(nameof(valueSyntax));
			}

			if (valueSyntax is NullValueNode)
			{
				return null;
			}
			if (converter.CanConvertFrom(valueSyntax.Value.GetType()))
				return converter.ConvertFrom(valueSyntax.Value);

			throw new ArgumentException(
				$"Cannot parse {valueSyntax.GetType().Name} to {typeof(TValue).Name}.",
				nameof(valueSyntax));
		}

		public override IValueNode ParseResult(object resultValue)
		{

			return ParseValue(resultValue);
		}

		// define how a native type is parsed into a literal,
		public override IValueNode ParseValue(object value)
		{
			if (value == null)
			{
				return new NullValueNode(null);
			}

			switch (value)
			{
				case string s:
					return new StringValueNode(null, s, false);
				case char c:
					return new StringValueNode(null, c.ToString(), false);
				case int i:
					return new IntValueNode(i);
				case long l:
					return new IntValueNode(l);
				case byte b:
					return new IntValueNode(b);
				case short sh:
					return new IntValueNode(sh);
			}

			throw new ArgumentException(
				"The specified value cannot be converted to value node ");
		}

		// define the result serialization. A valid output must be of the following .NET types:
		// System.String, System.Char, System.Int16, System.Int32, System.Int64,
		// System.Float, System.Double, System.Decimal and System.Boolean
		public override object Serialize(object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is IStrongTypeId strongTypeValue)
				return strongTypeValue.GetValue();

			throw new ArgumentException(
				"The specified value cannot be serialized by the StrongTypeId.");
		}

		public override bool TryDeserialize(object serialized, out object value)
		{
			if (serialized is null)
			{
				value = null;
				return true;
			}

			if (serialized is string s)
			{
				value = s;
				return true;
			}

			value = null;
			return false;
		}

		public override bool TrySerialize(object runtimeValue, out object resultValue)
		{
			if (runtimeValue is IStrongTypeId strongTypeValue)
			{
				resultValue = strongTypeValue.GetValue();
				return true;
			}
			resultValue = null;
			return false;
		}
	}
}
