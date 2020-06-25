using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class AnimeBuilder
	{
		private string _title = string.Empty;
		private string _imageUrl = string.Empty;
		private string _airingDate;
		private string _japaneseTitle = string.Empty;
		private string _titleSynonyms = string.Empty;
		private string _about = string.Empty;
		private long _malId;

		private AnimeType _animeType;
		private AnimeStatus _animeStatus;

		private AnimeTypeBuilder _animeTypeBuilder;
		private AnimeStatusBuilder _animeStatusBuilder;

		public Anime Build() => new Anime
		{
			Title = _title,
			ImageUrl = _imageUrl,
			MalId = _malId,
			AiringDate = _airingDate,
			JapaneseTitle = _japaneseTitle,
			TitleSynonyms = _titleSynonyms,
			About = _about,
			Status = _animeStatusBuilder?.Build() ?? _animeStatus,
			Type = _animeTypeBuilder?.Build() ?? _animeType
		};

		public AnimeBuilder WithTitle(string title)
		{
			_title = title;
			return this;
		}

		public AnimeBuilder WithImageUrl(string imageUrl)
		{
			_imageUrl = imageUrl;
			return this;
		}

		public AnimeBuilder WithMalId(long malId)
		{
			_malId = malId;
			return this;
		}

		public AnimeBuilder WithAiringDate(string airingDate)
		{
			_airingDate = airingDate;
			return this;
		}

		public AnimeBuilder WithJapaneseTitle(string japanesetitle)
		{
			_japaneseTitle = japanesetitle;
			return this;
		}

		public AnimeBuilder WithTitleSynonyms(string titleSynonyms)
		{
			_titleSynonyms = titleSynonyms;
			return this;
		}

		public AnimeBuilder WithAbout(string about)
		{
			_about = about;
			return this;
		}

		public AnimeBuilder WithAnimeType(AnimeTypeBuilder animeTypeBuilder)
		{
			_animeType = animeTypeBuilder.Build();
			return this;
		}

		public AnimeBuilder WithAnimeType(Action<AnimeTypeBuilder> builderAction)
		{
			_animeTypeBuilder = new AnimeTypeBuilder();
			builderAction(_animeTypeBuilder);
			return this;
		}

		public AnimeBuilder WithAnimeStatus(Action<AnimeStatusBuilder> builderAction)
		{
			_animeStatusBuilder = new AnimeStatusBuilder();
			builderAction(_animeStatusBuilder);
			return this;
		}

		public AnimeBuilder WithAnimeStatus(AnimeStatus animeStatus)
		{
			_animeStatus = animeStatus;
			return this;
		}
	}
}