using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class LanguageBuilder
	{
		private string _description;
		private int _id;

		public Language Build() => new Language
		{
			Description = _description,
			Id = _id
		};

		public LanguageBuilder WithDescription(string description)
		{
			_description = description;
			return this;
		}

		public LanguageBuilder WithId(int id)
		{
			_id = id;
			return this;
		}
	}
}