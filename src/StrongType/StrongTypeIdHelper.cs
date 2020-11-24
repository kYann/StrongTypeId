using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace StrongType
{
    public static class StrongTypeIdHelper
    {
        private static readonly ConcurrentDictionary<Type, Delegate> StrongTypeIdFactories = new();

        public static Func<TValue, object> GetFactory<TValue>(Type strongTypeIdType)
            where TValue : notnull
        {
            return (Func<TValue, object>)StrongTypeIdFactories.GetOrAdd(
                strongTypeIdType,
                CreateFactory<TValue>);
        }

        private static Func<TValue, object> CreateFactory<TValue>(Type strongTypeIdType)
            where TValue : notnull
        {
            if (!IsStrongTypeId(strongTypeIdType))
                throw new ArgumentException($"Type '{strongTypeIdType}' is not a strong type id type", nameof(strongTypeIdType));

            var ctor = strongTypeIdType.GetConstructor(new[] { typeof(TValue) });
            if (ctor is null)
                throw new ArgumentException($"Type '{strongTypeIdType}' doesn't have a constructor with one parameter of type '{typeof(TValue)}'", nameof(strongTypeIdType));

            var param = Expression.Parameter(typeof(TValue), "value");
            var body = Expression.New(ctor, param);
            var lambda = Expression.Lambda<Func<TValue, object>>(body, param);
            return lambda.Compile();
        }

        public static bool IsStrongTypeId(Type type) => IsStrongTypeId(type, out _);

        public static bool IsStrongTypeId(Type type, [NotNullWhen(true)] out Type idType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.BaseType is Type baseType &&
                baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(StrongTypeId<>))
            {
                idType = baseType.GetGenericArguments()[0];
                return true;
            }

            idType = null;
            return false;
        }
    }
}
