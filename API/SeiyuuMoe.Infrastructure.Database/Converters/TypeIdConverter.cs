using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeiyuuMoe.Domain.ValueObjects.Base;
using System;

namespace SeiyuuMoe.Infrastructure.Database.Converters
{
	public class TypeIdConverter : ValueConverter<TypeId, Guid>
	{
		public TypeIdConverter(ConverterMappingHints mappingHints = null)
			: base(
				  id => id.Value,
				  value => new TypeId(value),
				  mappingHints
				)
		{ }
	}
}