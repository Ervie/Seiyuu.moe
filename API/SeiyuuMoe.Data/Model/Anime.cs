using System;
using System.Collections.Generic;

namespace SeiyuuMoe.Data.Model
{
    public partial class Anime
    {
        public Anime()
        {
            Role = new HashSet<Role>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public long MalId { get; set; }
        public string ImageUrl { get; set; }
        public long? Popularity { get; set; }
        public string JapaneseTitle { get; set; }
        public string TitleSynonyms { get; set; }
        public string About { get; set; }
        public string AiringDate { get; set; }
        public long? StatusId { get; set; }
        public long? TypeId { get; set; }
        public long? SeasonId { get; set; }

        public virtual Season Season { get; set; }
        public virtual AnimeStatus Status { get; set; }
        public virtual AnimeType Type { get; set; }
        public virtual ICollection<Role> Role { get; set; }
    }
}
