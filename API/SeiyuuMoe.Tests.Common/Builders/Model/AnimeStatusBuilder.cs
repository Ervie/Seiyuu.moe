using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class AnimeStatusBuilder
	{
		private string _name = string.Empty;
		private AnimeStatusId _id;

		public AnimeStatus Build() 
			=> new AnimeStatus 
			{ 
				Id = _id,
				Description = _name
			};

		public AnimeStatusBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public AnimeStatusBuilder WithId(AnimeStatusId id)
		{
			_id = id;
			return this;
		}
	}
}