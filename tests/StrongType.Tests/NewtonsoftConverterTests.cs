using Newtonsoft.Json;
using StrongType.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace StrongType.Tests
{
	public class NewtonsoftConverterTests
	{
		public record GuidTests(Guid Value) : StrongTypeId<Guid>(Value);

		[Fact]
		public void WhenSerializingStrongTypeIdThenSerializationIsLikeUnderlyingType()
		{
			var id = Guid.NewGuid();

			var strongTypeId = new GuidTests(id);

			var settings = new JsonSerializerSettings()
			{
				Converters = new List<JsonConverter>() { new StrongTypeIdJsonConverter() }
			};
			

			var serializedId = JsonConvert.SerializeObject(id, settings);
			var serializedStrongTypeId = JsonConvert.SerializeObject(strongTypeId, settings);

			Assert.Equal(serializedId, serializedStrongTypeId);
		}

		[Fact]
		public void WhenDeserializingIdToStrongTypeIdThenDeserializationWorks()
		{
			var id = Guid.NewGuid();

			var settings = new JsonSerializerSettings()
			{
				Converters = new List<JsonConverter>() { new StrongTypeIdJsonConverter() }
			};


			var serializedId = JsonConvert.SerializeObject(id, settings);
			var serializedStrongTypeId = JsonConvert.DeserializeObject<GuidTests>(serializedId, settings);

			Assert.Equal(id, serializedStrongTypeId.Value);
		}
	}
}
