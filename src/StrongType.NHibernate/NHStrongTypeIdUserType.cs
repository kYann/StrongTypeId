using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using System;
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
				return new[] { (SqlType)TypeFactory.GetDefaultTypeFor(idType) };
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
