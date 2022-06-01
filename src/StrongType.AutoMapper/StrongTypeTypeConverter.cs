using AutoMapper;

namespace StrongType.AutoMapper;

public class StrongTypeTypeToValueConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    where TSource : StrongTypeId<TDestination> where TDestination : notnull
{
    public TDestination Convert(TSource source, TDestination destination, ResolutionContext context)
    {
        return source.Value;
    }

    public static Type MakeType(Type strongTypeIdType, Type strongTypeIdValueType)
    {
        return typeof(StrongTypeTypeToValueConverter<,>).MakeGenericType(strongTypeIdType, strongTypeIdValueType);
    }
}

public static class StrongTypeTypeToValueConverter
{
    public static Type MakeType(Type strongTypeIdType, Type strongTypeIdValueType)
    {
        return typeof(StrongTypeTypeToValueConverter<,>).MakeGenericType(strongTypeIdType, strongTypeIdValueType);
    }
}

public class ValueToStrongTypeTypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    where TDestination : StrongTypeId<TSource> where TSource : notnull
{
    StrongTypeIdConverter converter = new StrongTypeIdConverter(typeof(TDestination));

    public ValueToStrongTypeTypeConverter()
    {
        var isAuthorized = converter.CanConvertFrom(typeof(TSource));
    }

    public TDestination Convert(TSource source, TDestination destination, ResolutionContext context)
    {
        return (TDestination)converter.ConvertFrom(source);
    }
}

public static class ValueToStrongTypeTypeConverter
{
    public static Type MakeType(Type strongTypeIdValueType, Type strongTypeIdType)
    {
        return typeof(ValueToStrongTypeTypeConverter<,>).MakeGenericType(strongTypeIdValueType, strongTypeIdType);
    }
}