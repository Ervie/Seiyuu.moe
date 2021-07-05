using System;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public partial class AnimeRole
	{
		[Key]
		public Guid Id { get; set; }
		public Guid? AnimeId { get; set; }
		public AnimeRoleTypeId? RoleTypeId { get; set; }
		public Guid? CharacterId { get; set; }
		public Guid? SeiyuuId { get; set; }
		public LanguageId? LanguageId { get; set; }

		public virtual Anime Anime { get; set; }
		public virtual AnimeCharacter Character { get; set; }
		public virtual AnimeRoleType RoleType { get; set; }
		public virtual Seiyuu Seiyuu { get; set; }
		public virtual Language Language { get; set; }
	}
}