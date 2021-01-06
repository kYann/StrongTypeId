using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace StrongType.EFCore
{
	public class StrongTypeIdValueConverter<TStrongType, TValue> : ValueConverter<TStrongType, TValue> where TStrongType : StrongTypeId<TValue>
	{
		static readonly Func<TValue, object> factory = StrongTypeIdHelper.GetFactory<TValue>(typeof(TStrongType));

		public StrongTypeIdValueConverter() : base((id) => id.Value, (id) => (TStrongType)factory(id))
		{
		}
	}
}
