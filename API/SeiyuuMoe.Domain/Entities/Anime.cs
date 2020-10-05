using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class Anime
	{
		public Anime()
		{
			Role = new HashSet<AnimeRole>();
		}

		public string Title { get; set; }

		[Key]
		public Guid Id { get; set; }

		public long MalId { get; set; }
		public string ImageUrl { get; set; }
		public long? Popularity { get; set; }
		public string EnglishTitle { get; set; }
		public string KanjiTitle { get; set; }
		public string TitleSynonyms { get; set; }
		public string About { get; set; }
		public DateTime AiringDate { get; set; }
		public long? StatusId { get; set; }
		public long? TypeId { get; set; }
		public long? SeasonId { get; set; }
		public DateTime? ModificationDate { get; set; }

		public virtual AnimeSeason Season { get; set; }
		public virtual AnimeStatus Status { get; set; }
		public virtual AnimeType Type { get; set; }
		public virtual ICollection<AnimeRole> Role { get; set; }
	}
}