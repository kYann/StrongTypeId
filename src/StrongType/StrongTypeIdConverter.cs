using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;

namespace StrongType
{
    public class StrongTypeIdConverter<TValue> : TypeConverter
    where TValue : notnull
    {
        private static readonly TypeConverter IdValueConverter = GetIdValueConverter();

        private static TypeConverter GetIdValueConverter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(TValue));
            if (!converter.CanConvertFrom(typeof(string)))
                throw new InvalidOperationException(
                    $"Type '{typeof(TValue)}' doesn't have a converter that can convert from string");
            return converter;
        }

        private readonly Type _type;
        public StrongTypeIdConverter(Type type)
        {
            _type = type;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                || sourceType == typeof(TValue)
                || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string)
                || destinationType == typeof(TValue)
                || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (typeof(TValue) != typeof(string) && value is string s)
            {
                value = IdValueConverter.ConvertFrom(s);
            }
            if (value is TValue idValue)
            {
                var factory = StrongTypeIdHelper.GetFactory<TValue>(_type);
                return factory(idValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            var strongTypeId = (StrongTypeId<TValue>)value;
            TValue idValue = strongTypeId.Value;
            if (destinationType == typeof(string))
                return idValue.ToString()!;
            if (destinationType == typeof(TValue))
                return idValue;
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class StrongTypeIdConverter : TypeConverter
    {
        private static readonly ConcurrentDictionary<Type, TypeConverter> ActualConverters = new();

        private readonly TypeConverter _innerConverter;

        public StrongTypeIdConverter(Type strongTypeIdType)
        {
            _innerConverter = ActualConverters.GetOrAdd(strongTypeIdType, CreateActualConverter);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            _innerConverter.CanConvertFrom(context, sourceType);
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            _innerConverter.CanConvertTo(context, destinationType);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            _innerConverter.ConvertFrom(context, culture, value);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            _innerConverter.ConvertTo(context, culture, value, destinationType);


        private static TypeConverter CreateActualConverter(Type strongTypeIdType)
        {
            if (!StrongTypeIdHelper.IsStrongTypeId(strongTypeIdType, out var idType))
                throw new InvalidOperationException($"The type '{strongTypeIdType}' is not a strong type id");

            var actualConverterType = typeof(StrongTypeIdConverter<>).MakeGenericType(idType);
            return (TypeConverter)Activator.CreateInstance(actualConverterType, strongTypeIdType)!;
        }
    }
}
