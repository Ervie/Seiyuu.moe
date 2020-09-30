using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Common.Builders.Model
{
	public class AnimeBuilder
	{
		private string _title = string.Empty;
		private string _imageUrl = string.Empty;
		private DateTime _airingDate = DateTime.MinValue;
		private string _japaneseTitle = string.Empty;
		private string _englishTitle = string.Empty;
		private string _titleSynonyms = string.Empty;
		private string _about = string.Empty;

		private int _popularity;

		private long _malId;
		private Guid _id;

		private AnimeType _animeType;
		private AnimeStatus _animeStatus;
		private AnimeSeason _season;

		private AnimeTypeBuilder _animeTypeBuilder;
		private AnimeStatusBuilder _animeStatusBuilder;
		private SeasonBuilder _seasonBuilder;

		public Anime Build() => new Anime
		{
			Id = _id,
			Title = _title,
			ImageUrl = _imageUrl,
			MalId = _malId,
			AiringDate = _airingDate,
			KanjiTitle = _japaneseTitle,
			EnglishTitle = _englishTitle,
			TitleSynonyms = _titleSynonyms,
			About = _about,
			Popularity = _popularity,
			Status = _animeStatusBuilder?.Build() ?? _animeStatus,
			Type = _animeTypeBuilder?.Build() ?? _animeType,
			Season = _seasonBuilder?.Build() ?? _season
		};

		public AnimeBuilder WithId(Guid id)
		{
			_id = id;
			return this;
		}

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

		public AnimeBuilder WithAiringDate(DateTime airingDate)
		{
			_airingDate = airingDate;
			return this;
		}

		public AnimeBuilder WithJapaneseTitle(string japanesetitle)
		{
			_japaneseTitle = japanesetitle;
			return this;
		}

		public AnimeBuilder WithEnglishTitle(string englishTitle)
		{
			_englishTitle = englishTitle;
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

		public AnimeBuilder WithPopularity(int popularity)
		{
			_popularity = popularity;
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

		public AnimeBuilder WithSeason(AnimeSeason season)
		{
			_season = season;
			return this;
		}

		public AnimeBuilder WithSeason(Action<SeasonBuilder> builderAction)
		{
			_seasonBuilder = new SeasonBuilder();
			builderAction(_seasonBuilder);
			return this;
		}
	}
}