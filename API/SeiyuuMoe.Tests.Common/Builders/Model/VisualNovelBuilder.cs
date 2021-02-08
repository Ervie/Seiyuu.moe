using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class VisualNovelBuilder
	{
		private string _title = string.Empty;
		private string _imageUrl = string.Empty;
		private string _japaneseTitle = string.Empty;
		private string _titleSynonyms = string.Empty;
		private string _about = string.Empty;

		private int _popularity;

		private long _vndbId;
		private Guid _id;

		public VisualNovel Build() => new VisualNovel
		{
			Id = _id,
			Title = _title,
			ImageUrl = _imageUrl,
			VndbId = _vndbId,
			TitleOriginal = _japaneseTitle,
			Alias = _titleSynonyms,
			About = _about,
			Popularity = _popularity
		};

		public VisualNovelBuilder WithId(Guid id)
		{
			_id = id;
			return this;
		}

		public VisualNovelBuilder WithTitle(string title)
		{
			_title = title;
			return this;
		}

		public VisualNovelBuilder WithImageUrl(string imageUrl)
		{
			_imageUrl = imageUrl;
			return this;
		}

		public VisualNovelBuilder WithVndbId(long vndbId)
		{
			_vndbId = vndbId;
			return this;
		}

		public VisualNovelBuilder WithJapaneseTitle(string japanesetitle)
		{
			_japaneseTitle = japanesetitle;
			return this;
		}

		public VisualNovelBuilder WithTitleSynonyms(string titleSynonyms)
		{
			_titleSynonyms = titleSynonyms;
			return this;
		}

		public VisualNovelBuilder WithAbout(string about)
		{
			_about = about;
			return this;
		}

		public VisualNovelBuilder WithPopularity(int popularity)
		{
			_popularity = popularity;
			return this;
		}
	}
}