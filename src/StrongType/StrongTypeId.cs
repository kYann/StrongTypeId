using System;
using System.ComponentModel;

namespace StrongType
{
    public interface IStrongTypeId
    {
		object GetValue();
    }

    [TypeConverter(typeof(StrongTypeIdConverter))]
    public abstract record StrongTypeId<TValue>(TValue Value) : IStrongTypeId where TValue : notnull
    {
        public object GetValue() => Value;

		public override string ToString() => Value.ToString();

		public static bool operator ==(StrongTypeId<TValue> a, TValue b)
		{
			if (a is null)
				return b is null;
			return a.Value.Equals(b);
		}

		public static bool operator !=(StrongTypeId<TValue> a, TValue b)
		{
			if (a is null)
				return !(b is null);
			return !a.Value.Equals(b);
		}

		public static bool operator ==(TValue b, StrongTypeId<TValue> a)
		{
			if (a is null)
				return b is null;
			return a.Value.Equals(b);
		}

		public static bool operator !=(TValue b, StrongTypeId<TValue> a)
		{
			if (a is null)
				return !(b is null);
			return !a.Value.Equals(b);
		}

		public static explicit operator TValue(StrongTypeId<TValue> a)
		{
			if (a is null)
				return default(TValue);
			return a.Value;
		}

		public bool Equals(TValue value)
		{
			return this.Value.Equals(value);
		}
	}
}
