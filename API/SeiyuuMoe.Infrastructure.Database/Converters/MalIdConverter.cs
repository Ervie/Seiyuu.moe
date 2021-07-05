using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeiyuuMoe.Domain.ValueObjects;

namespace SeiyuuMoe.Infrastructure.Database.Converters
{
	public class MalIdConverter : ValueConverter<MalId, long>
	{
		public MalIdConverter(ConverterMappingHints mappingHints = null)
			: base(
				  id => id.Value,
				  value => new MalId(value),
				  mappingHints
				)
		{ }
	}
}