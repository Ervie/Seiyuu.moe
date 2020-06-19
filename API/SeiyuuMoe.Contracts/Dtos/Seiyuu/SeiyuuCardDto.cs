using System;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class SeiyuuCardDto
	{
		public long MalId { get; }

		public string Name { get; }

		public string ImageUrl { get; }

		public string JapaneseName { get; }

		public DateTime? Birthday { get; }

		public string About { get; }

		public SeiyuuCardDto(long malId, string name, string imageUrl, string japaneseName, DateTime? birthday, string about)
		{
			MalId = malId;
			Name = name;
			ImageUrl = imageUrl;
			JapaneseName = japaneseName;
			Birthday = birthday;
			About = about;
		}
	}
}