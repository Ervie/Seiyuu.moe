using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class LanguageBuilder
	{
		private string _name;
		private int _id;

		public Language Build() => new Language
		{
			Name = _name,
			Id = _id
		};

		public LanguageBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public LanguageBuilder WithId(int id)
		{
			_id = id;
			return this;
		}
	}
}