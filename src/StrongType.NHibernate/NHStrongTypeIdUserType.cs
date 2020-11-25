using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using System;
using System.Data;
using System.Data.Common;

namespace StrongType.NHibernate
{
	[Serializable]
	public class NHStrongTypeIdUserType<T> : IUserType where T : IStrongTypeId
	{
		public SqlType[] SqlTypes
		{
			get
			{
				StrongTypeIdHelper.IsStrongTypeId(typeof(T), out var idType);
				var defaultType = TypeFactory.GetDefaultTypeFor(idType);
				var dbType = (defaultType) switch
				{
					GuidType => DbType.Guid,
					Int16Type => DbType.Int16,
					Int32Type => DbType.Int32,
					Int64Type => DbType.Int64,
					StringType => DbType.String,
					AnsiStringType => DbType.AnsiString,
					AnsiCharType => DbType.AnsiStringFixedLength,
					BinaryType => DbType.Binary,
					BooleanType => DbType.Boolean,
					ByteType => DbType.Byte,
					CurrencyType => DbType.Currency,
					DateType => DbType.Date,
					DateTimeType => DbType.DateTime,
					DateTimeOffsetType => DbType.DateTimeOffset,
					DecimalType => DbType.Decimal,
					DoubleType => DbType.Double,
					SByteType => DbType.SByte,
					SingleType => DbType.Single,
					TimeType => DbType.Time,
					UInt16Type => DbType.UInt16,
					UInt32Type => DbType.UInt32,
					UInt64Type => DbType.UInt64,
					_ => throw new InvalidOperationException($"Cannot find DbType for {defaultType}")

				};
				return new SqlType[] { new SqlType(dbType) };
			}
		}

		public System.Type ReturnedType
		{
			get { return typeof(T); }
		}

		public new bool Equals(object x, object y)
		{
			if (x == null)
			{
				return y == null;
			}
			else
			{
				return x.Equals(y);
			}
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		private IStrongTypeId GenerateStrongType(object value)
		{
			var strongType = Activator.CreateInstance(typeof(T), value) as IStrongTypeId;

			return strongType;
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var ordinal = rs.GetOrdinal(names[0]);
			var value = rs.GetValue(ordinal);
			if (value == DBNull.Value)
				return null;

			return GenerateStrongType(value);
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (value == null)
			{
				cmd.Parameters[index].Value = DBNull.Value;
				return;
			}
			if (value is IStrongTypeId stValue)
			{
				var realValue = stValue.GetValue();
				var nhType = NHibernateUtil.GuessType(realValue);
				nhType.NullSafeSet(cmd, realValue, index, session);
			}
			else
			{
				var nhType = NHibernateUtil.GuessType(value);
				nhType.NullSafeSet(cmd, value, index, session);
			}
		}

		public object DeepCopy(object value)
		{
			if (value == null) return null;
			var realValue = (value as IStrongTypeId).GetValue();
			return GenerateStrongType(realValue);
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}
	}
}
