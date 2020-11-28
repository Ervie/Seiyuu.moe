namespace SeiyuuMoe.Domain.MalUpdateData
{
	public class MalCharacterUpdateData
	{
		public string Name { get; private set; }

		public string About { get; private set; }

		public string JapaneseName { get; private set; }

		public string ImageUrl { get; private set; }

		public string Nicknames { get; private set; }

		public int? Popularity { get; private set; }

		public MalCharacterUpdateData(string name, string about, string japaneseName, string imageUrl, string nicknames, int? popularity)
		{
			Name = name;
			About = about;
			JapaneseName = japaneseName;
			ImageUrl = imageUrl;
			Nicknames = nicknames;
			Popularity = popularity;
		}
	}
}