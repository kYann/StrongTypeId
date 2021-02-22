using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace StrongType.EFCore
{
    public class StrongTypeIdValueConverter<TStrongType, TValue> : ValueConverter<TStrongType, TValue> where TStrongType : StrongTypeId<TValue>
    {
        static readonly Func<TValue, object> factory = StrongTypeIdHelper.GetFactory<TValue>(typeof(TStrongType));

        public StrongTypeIdValueConverter() : base((id) => id.Value, (id) => (TStrongType)factory(id), new ConverterMappingHints(valueGeneratorFactory: (p, t) => new TemporaryTypedIdValueGenerator<TStrongType, TValue>()))
        {
        }
    }

    public class TemporaryTypedIdValueGenerator<TTypedId, TValue> : TemporaryNumberValueGenerator<TTypedId>
    {
        private int current = int.MinValue + 1000;

        public override TTypedId Next(EntityEntry entry)
        {
            if (typeof(TValue) == typeof(Guid))
                return (TTypedId)Activator.CreateInstance(typeof(TTypedId), Guid.NewGuid());
            else if (typeof(TValue) == typeof(int))
                return (TTypedId)Activator.CreateInstance(typeof(TTypedId), Interlocked.Increment(ref this.current));
            else
                return (TTypedId)Activator.CreateInstance(typeof(TTypedId), default(TValue));
        }
    }
}
