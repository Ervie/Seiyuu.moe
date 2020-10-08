using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class LanguageBuilder
	{
		private string _description;
		private LanguageId _languageId;

		public Language Build() => new Language
		{
			Description = _description,
			Id = _languageId
		};

		public LanguageBuilder WithDescription(string description)
		{
			_description = description;
			return this;
		}

		public LanguageBuilder WithLanguageId(LanguageId languageId)
		{
			_languageId = languageId;
			return this;
		}
	}
}