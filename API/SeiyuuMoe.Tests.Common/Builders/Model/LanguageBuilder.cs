using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class LanguageBuilder
	{
		private LanguageId _languageId;
		private string _description = "Japanese";

		public Language Build() => new Language
		{
			Id = _languageId,
			Description = _description
		};

		public LanguageBuilder WithLanguageId(LanguageId languageId)
		{
			_languageId = languageId;
			return this;
		}

		public LanguageBuilder WithDescription(string description)
		{
			_description = description;
			return this;
		}
	}
}