using System;
using System.ComponentModel;
using Xunit;

namespace StrongType.Tests
{
	public class StrongTypeIdTests
	{
		public record StringTests(string Value) : StrongTypeId<string>(Value)
        {
        }

		public record ValidateTests(string Value) : StrongTypeId<string>(Value)
		{
            public override void Validate(string value)
            {
				if (value.StartsWith('a') == false)
					throw new InvalidOperationException("Wrong primitive");
            }
        }

		[Fact]
		public void WhenTestingEqualityOperatorWithUnderlyingTypeThenItWorks()
		{
			var testString = "Test de string";
			var testStrong = new StringTests(testString);

			Assert.True(testString == testStrong);
			Assert.True(testStrong == testString);
		}

		[Fact]
		public void WhenTestingNotEqualityOperatorWithUnderlyingTypeThenItWorks()
		{
			var testString = "Test de string";
			var testStrong = new StringTests(testString);

			Assert.False(testString != testStrong);
			Assert.False(testStrong != testString);
		}

		[Fact]
		public void WhenTestingEqualityWithUnderlyingTypeThenItWorks()
		{
			var testString = "Test de string";
			var testStrong = new StringTests(testString);

			Assert.True(testStrong.Equals(testString));
		}

		[Fact]
		public void WhenTestingCastToUnderlyingTypeThenItWorks()
		{
			var testString = "Test de string";
			var testStrong = new StringTests(testString);

			var resultString = (string)testStrong;

			Assert.Equal(testString, resultString);
		}

		[Fact]
		public void WhenTestingCastFromUnderlyingTypeThenItWorks()
		{
			var testString = "Test de string";

			var resultStrong = new StringTests(testString);

			Assert.Equal(testString, resultStrong.Value);
		}

		[Fact]
		public void WhenTestingEqualityOperatorThenItWorks()
		{
			var testStrong1 = new StringTests("Test de string");
			var testStrong2 = new StringTests("Test de string");

			Assert.True(testStrong1 == testStrong2);
		}

		[Fact]
		public void WhenTestingNotEqualityOperatorThenItWorks()
		{
			var testStrong1 = new StringTests("Test de string");
			var testStrong2 = new StringTests("Test de string");

			Assert.False(testStrong1 != testStrong2);
		}

		[Fact]
		public void WhenTestingNullThenItWorks()
		{
			var testStrong1 = (StringTests)null;
			var testStrong2 = new StringTests("Test de string");
			var testStrong3 = (StringTests)null;

			var testString1 = (string)null;

			Assert.True(testStrong1 != testStrong2);
			Assert.False(testStrong1 == testStrong2);
			Assert.True(testStrong2 != testStrong1);
			Assert.False(testStrong2 == testStrong1);

			Assert.True(testStrong1 == testStrong3);
			Assert.False(testStrong1 != testStrong3);

			Assert.True(testString1 != testStrong2);
			Assert.False(testString1 == testStrong2);
			Assert.True(testStrong2 != testString1);
			Assert.False(testStrong2 == testString1);
		}

		[Fact]
		public void WhenTestingNotValidStringThenItWorks()
		{
			Assert.Throws<InvalidOperationException>(() =>
				new ValidateTests("Test de string")
			);
		}

		[Fact]
		public void WhenTestingValidStringThenItWorks()
		{
			new ValidateTests("a aaa aa");
		}

		[Fact]
		public void WhenTestingConverterThenItWorks()
		{
			var converter = TypeDescriptor.GetConverter(typeof(StringTests));
			var convertedValue = converter.ConvertFrom("test value");
			Assert.IsType<StringTests>(convertedValue);
			Assert.Equal("test value", ((StringTests)convertedValue).Value);
		}
	}
}
