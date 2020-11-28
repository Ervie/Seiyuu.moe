using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class LanguageBuilder
	{
		private LanguageId _languageId;

		public Language Build() => new Language
		{
			Id = _languageId
		};

		public LanguageBuilder WithLanguageId(LanguageId languageId)
		{
			_languageId = languageId;
			return this;
		}
	}
}