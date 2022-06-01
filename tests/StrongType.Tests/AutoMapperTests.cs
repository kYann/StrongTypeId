using AutoMapper;
using StrongType.AutoMapper;
using System;
using Xunit;

namespace StrongType.Tests
{
    public class AutoMapperTests
    {
        public record GuidTests(Guid Value) : StrongTypeId<Guid>(Value);

        [Fact]
        public void WhenConvertingStrongTypeToValue_ThenItWorks()
        {
            var config = new MapperConfiguration(cfg => cfg.MapStrongTypeId(typeof(GuidTests), typeof(Guid)));
            var mapper = config.CreateMapper();

            var guid = Guid.NewGuid();
            var value = mapper.Map<Guid>(new GuidTests(guid));

            Assert.Equal(guid, value);
        }

        [Fact]
        public void WhenConvertingValueToStrongType_ThenItWorks()
        {
            var config = new MapperConfiguration(cfg => cfg.MapStrongTypeId(typeof(GuidTests), typeof(Guid)));
            var mapper = config.CreateMapper();

            var guid = Guid.NewGuid();
            var value = mapper.Map<GuidTests>(guid);

            Assert.Equal(guid, value.Value);
        }
    }
}
