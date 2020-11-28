using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Domain.MalUpdateData
{
	public class MalSeiyuuUpdateData
	{
		public string Name { get; private set; }

		public string About { get; private set; }

		public string JapaneseName { get; private set; }

		public string ImageUrl { get; private set; }

		public int? Popularity { get; private set; }

		public DateTime? Birthday { get; private set; }

		public ICollection<MalVoiceActingRoleUpdateData> VoiceActingRoles { get; private set; }

		public MalSeiyuuUpdateData(string name, string about, string japaneseName, string imageUrl, int? popularity, DateTime? birthday, ICollection<MalVoiceActingRoleUpdateData> voiceActingRoles)
		{
			Name = name;
			About = about;
			JapaneseName = japaneseName;
			ImageUrl = imageUrl;
			Popularity = popularity;
			Birthday = birthday;
			VoiceActingRoles = voiceActingRoles;
		}
	}
}