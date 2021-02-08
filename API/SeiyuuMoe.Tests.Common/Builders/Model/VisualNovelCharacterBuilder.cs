using SeiyuuMoe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class VisualNovelCharacterBuilder
	{
		private string _name = string.Empty;
		private string _imageUrl = string.Empty;
		private string _nameKanji = string.Empty;
		private string _about = string.Empty;
		private string _nicknames = string.Empty;
		private long _vndbId;
		private Guid _id;

		public VisualNovelCharacter Build() => new VisualNovelCharacter
		{
			Id = _id,
			Name = _name,
			VndbId = _vndbId,
			ImageUrl = _imageUrl,
			KanjiName = _nameKanji,
			Nicknames = _nicknames,
			About = _about
		};

		public VisualNovelCharacterBuilder WithId(Guid id)
		{
			_id = id;
			return this;
		}

		public VisualNovelCharacterBuilder WithName(string name)
		{
			_name = name;
			return this;
		}

		public VisualNovelCharacterBuilder WithImageUrl(string imageUrl)
		{
			_imageUrl = imageUrl;
			return this;
		}

		public VisualNovelCharacterBuilder WithVndbId(long vndbId)
		{
			_vndbId = vndbId;
			return this;
		}

		public VisualNovelCharacterBuilder WithKanjiName(string kanjiName)
		{
			_nameKanji = kanjiName;
			return this;
		}

		public VisualNovelCharacterBuilder WithAbout(string about)
		{
			_about = about;
			return this;
		}

		public VisualNovelCharacterBuilder WithNicknames(string nicknames)
		{
			_nicknames = nicknames;
			return this;
		}
	}
}
