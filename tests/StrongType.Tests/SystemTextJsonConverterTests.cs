using StrongType.System.Text.Json;
using System;
using System.Text.Json;
using Xunit;

namespace StrongType.Tests
{
	public class SystemTextJsonConverterTests
	{
		public record GuidTests(Guid Value) : StrongTypeId<Guid>(Value);

        public record StringTests(string Value) : StrongTypeId<string>(Value);

        [Fact]
		public void WhenSerializingStrongTypeIdThenSerializationIsLikeUnderlyingType()
		{
			var id = Guid.NewGuid();

			var strongTypeId = new GuidTests(id);

			var serializeOptions = new JsonSerializerOptions();
			serializeOptions.Converters.Add(new StrongTypeIdJsonConverterFactory());

			var serializedId = JsonSerializer.Serialize(id, serializeOptions);
			var serializedStrongTypeId = JsonSerializer.Serialize(strongTypeId, serializeOptions);

			Assert.Equal(serializedId, serializedStrongTypeId);
		}

		[Fact]
		public void WhenDeserializingIdToStrongTypeIdThenDeserializationWorks()
		{
			var id = Guid.NewGuid();

			var serializeOptions = new JsonSerializerOptions();
			serializeOptions.Converters.Add(new StrongTypeIdJsonConverterFactory());


			var serializedId = JsonSerializer.Serialize(id, serializeOptions);
			var serializedStrongTypeId = JsonSerializer.Deserialize<GuidTests>(serializedId, serializeOptions);

			Assert.Equal(id, serializedStrongTypeId.Value);
		}

        [Fact]
        public void WhenSerializingStrongTypeIdStringThenSerializationIsLikeUnderlyingType()
        {
            var id = Guid.NewGuid().ToString();

            var strongTypeId = new StringTests(id);

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new StrongTypeIdJsonConverterFactory());

            var serializedId = JsonSerializer.Serialize(id, serializeOptions);
            var serializedStrongTypeId = JsonSerializer.Serialize(strongTypeId, serializeOptions);

            Assert.Equal(serializedId, serializedStrongTypeId);
        }

        [Fact]
        public void WhenDeserializingIdToStrongTypeIdStringThenDeserializationWorks()
        {
            var id = Guid.NewGuid().ToString();

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new StrongTypeIdJsonConverterFactory());


            var serializedId = JsonSerializer.Serialize(id, serializeOptions);
            var serializedStrongTypeId = JsonSerializer.Deserialize<StringTests>(serializedId, serializeOptions);

            Assert.Equal(id, serializedStrongTypeId.Value);
        }
    }
}
