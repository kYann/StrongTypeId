using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongType.System.Text.Json
{
	public class StrongTypeIdJsonConverterFactory : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert)
		{
			return StrongTypeIdHelper.IsStrongTypeId(typeToConvert);
		}

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
            StrongTypeIdHelper.IsStrongTypeId(typeToConvert, out var idType);
            var strongType = typeof(StrongTypeIdConverterInner<,>)
                .MakeGenericType(new Type[] { typeToConvert, idType });


            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                strongType,
				BindingFlags.Instance | BindingFlags.Public,
				binder: null,
				args: new object[] { options },
				culture: null);

			return converter;
		}

        private class StrongTypeIdConverterInner<TStrongType, TValue> : JsonConverter<TStrongType> where TValue : notnull where TStrongType : StrongTypeId<TValue>
        {
            private readonly JsonConverter<TValue> valueConverter;

            public StrongTypeIdConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
            }

            public override bool CanConvert(Type typeToConvert)
            {
                return StrongTypeIdHelper.IsStrongTypeId(typeToConvert);
            }

            public override TStrongType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var idValue = JsonSerializer.Deserialize<TValue>(ref reader, options);
                var converter = new StrongTypeIdConverter<TValue>(typeToConvert);
                var strongTypeId = converter.ConvertFrom(idValue);

                return (TStrongType)strongTypeId;
            }

            public override void Write(Utf8JsonWriter writer, TStrongType strongTypeId, JsonSerializerOptions options)
            {
                if (valueConverter is not null)
                {
                    valueConverter.Write(writer, strongTypeId.Value, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, strongTypeId.Value, options);
                }
            }
        }
    }
}
