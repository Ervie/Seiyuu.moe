using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Domain.ValueObjects.Base;
using System;
using Xunit;

namespace SeiyuuMoe.Tests.Domain.ValueObjects
{
	public class TypeIdTests
	{
		[Fact]
		public void CreateObject_GivenEmptyId_ShouldThrowValidationException()
		{
			// Given
			var guid = Guid.NewGuid();
			var TypeId = new TypeId(guid);
			var copyOfReference = TypeId;

			// When
			var areEqual = TypeId.Equals(copyOfReference);

			// Then
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void CompareTheSameInMemoryObjects_ShouldReturnTrue()
		{
			// Given
			var guid = Guid.NewGuid();
			var TypeId = new TypeId(guid);
			var copyOfReference = TypeId;

			// When
			var areEqual = TypeId.Equals(copyOfReference);

			// Then
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void CompareObjectsWithTheSameValue_ShouldReturnTrue()
		{
			// Given
			var guid = Guid.NewGuid();
			var typeId1 = new TypeId(guid);
			var typeId2 = new TypeId(guid);

			// When
			var areEqualFirst = typeId1.Equals(typeId2);
			var areEqualSecond = typeId2.Equals(typeId1);

			// Then
			using (new AssertionScope())
			{
				areEqualFirst.Should().BeTrue();
				areEqualSecond.Should().BeTrue();
			}
		}

		[Fact]
		public void CompareObjectsWithDifferentValue_ShouldReturnFalse()
		{
			// Given
			var guid1 = Guid.NewGuid();
			var guid2 = Guid.NewGuid();

			var typeId1 = new TypeId(guid1);
			var typeId2 = new TypeId(guid2);

			// When
			var areEqual = typeId1.Equals(typeId2);

			// Then
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void CompareWithNull_ShouldReturnFalse()
		{
			// Given
			var guid = Guid.NewGuid();
			var TypeId = new TypeId(guid);

			// When
			var areEqual = TypeId.Equals(null);

			// Then
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void CompareWithNullObject_ShouldReturnFalse()
		{
			// Given
			var guid = Guid.NewGuid();
			var TypeId = new TypeId(guid);

			// When
			var areEqual = TypeId.Equals((object?)null);

			// Then
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void HashCodeIsCorrect_ShouldGetCorrectHashCode()
		{
			// Given
			var guid = Guid.NewGuid();
			var TypeId = new TypeId(guid);
			var expectedHashCode = guid.GetHashCode();

			// When
			var actualHashCode = TypeId.GetHashCode();

			// Then
			actualHashCode.Should().Be(expectedHashCode);
		}

		[Fact]
		public void ToString_ShouldGetCorrectValue()
		{
			// Given
			var guid = Guid.NewGuid();
			var TypeId = new TypeId(guid);

			// When
			var stringValue = TypeId.ToString();

			// Then
			stringValue.Should().Be(guid.ToString());
		}
	}
}