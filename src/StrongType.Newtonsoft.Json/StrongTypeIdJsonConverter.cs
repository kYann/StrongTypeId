using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace StrongType.Newtonsoft.Json
{
	public class StrongTypeIdJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return StrongTypeIdHelper.IsStrongTypeId(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var converter = new StrongTypeIdConverter(objectType);
			var strongTypeId = converter.ConvertFrom(reader.Value);

			return strongTypeId;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var idValue = ((IStrongTypeId)value).GetValue();
			var token = JToken.FromObject(idValue);
			token.WriteTo(writer);
		}
	}
}
