using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class VisualNovelRoleBuilder
	{
		private Guid _characterId;
		private Guid _visualNovelId;
		private Guid _seiyuuId;

		public VisualNovelRole Build()
			=> new VisualNovelRole
			{
				CharacterId = _characterId,
				VisualNovelId = _visualNovelId,
				SeiyuuId = _seiyuuId
			};

		public VisualNovelRoleBuilder WithCharacterId(Guid characterId)
		{
			_characterId = characterId;
			return this;
		}

		public VisualNovelRoleBuilder WithVisualNovelId(Guid visualNovelId)
		{
			_visualNovelId = visualNovelId;
			return this;
		}

		public VisualNovelRoleBuilder WithSeiyuuId(Guid seiyuuId)
		{
			_seiyuuId = seiyuuId;
			return this;
		}
	}
}