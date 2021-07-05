using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Domain.Exceptions;
using SeiyuuMoe.Domain.ValueObjects;
using SeiyuuMoe.Tests.Common;
using System;
using Xunit;

namespace SeiyuuMoe.Tests.Domain.ValueObjects
{
	public class MalIdTests
	{
		[Theory]
		[InlineData(long.MinValue)]
		[InlineData(-1)]
		[InlineData(0)]
		public void CreateObject_GivenInvalidValue_ShouldThrowValidationException(long value)
		{
			// When
			Func<MalId> func = () => new MalId(value);

			// Then
			func.Should().ThrowExactly<ValidationException>();
		}

		[Fact]
		public void CreateObject_GivenValidValue_ShouldAssign()
		{
			// When
			var malId = new MalId(TestData.TestMalId);

			// Then
			malId.Value.Should().Be(TestData.TestMalId);
		}

		[Fact]
		public void CompareTheSameInMemoryObjects_ShouldReturnTrue()
		{
			// Given
			var contextId = new MalId(TestData.TestMalId);
			var copyOfReference = contextId;

			// When
			var areEqual = contextId.Equals(copyOfReference);

			// Then
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void CompareObjectsWithTheSameValue_ShouldReturnTrue()
		{
			// Given
			var malId1 = new MalId(TestData.TestMalId);
			var malId2 = new MalId(TestData.TestMalId);

			// When
			var areEqualFirst = malId1.Equals(malId2);
			var areEqualSecond = malId2.Equals(malId1);

			// Then
			using var scope = new AssertionScope();
			areEqualFirst.Should().BeTrue();
			areEqualSecond.Should().BeTrue();
		}

		[Fact]
		public void CompareObjectsWithDifferentValue_ShouldReturnFalse()
		{
			// Given
			var malId1 = new MalId(TestData.TestMalId);
			var malId2 = new MalId(TestData.TestMalId + 1);

			// When
			var areEqual = malId1.Equals(malId2);

			// Then
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void CompareWithNull_ShouldReturnFalse()
		{
			// Given
			var malId = new MalId(TestData.TestMalId);

			// When
			var areEqual = malId.Equals(null);

			// Then
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void CompareWithNullObject_ShouldReturnFalse()
		{
			// Given
			var malId = new MalId(TestData.TestMalId);

			// When
			var areEqual = malId.Equals((object?)null);

			// Then
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void ToString_ShouldGetCorrectValue()
		{
			// Given
			var malId = new MalId(TestData.TestMalId);

			// When
			var stringValue = malId.ToString();

			// Then
			stringValue.Should().Be(TestData.TestMalId.ToString());
		}
	}
}