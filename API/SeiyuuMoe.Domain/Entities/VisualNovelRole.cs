using System;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public class VisualNovelRole
	{
		[Key]
		public Guid Id { get; set; }

		public Guid? VisualNovelId { get; set; }
		public VisualNovelRoleTypeId? RoleTypeId { get; set; }
		public Guid? CharacterId { get; set; }
		public Guid? SeiyuuId { get; set; }
		public LanguageId? LanguageId { get; set; }

		public virtual VisualNovel VisualNovel { get; set; }
		public virtual VisualNovelCharacter Character { get; set; }
		public virtual VisualNovelRoleType RoleType { get; set; }
		public virtual Seiyuu Seiyuu { get; set; }
		public virtual Language Language { get; set; }
	}
}