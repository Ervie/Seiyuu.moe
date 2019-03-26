using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.ServBusinessServicesices.Mapper.Profiles
{
	internal class AnimeProfile : Profile
	{
		private const string malAnimeBaseUrl = "https://myanimelist.net/anime/";

		public AnimeProfile()
		{
			CreateMap<Anime, AnimeSearchEntryDto>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));


			CreateMap<Anime, AnimeCardDto>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.About, opt => opt.MapFrom(src => src.About))
				.ForMember(dest => dest.AiringDate, opt => opt.MapFrom(src => System.DateTime.Parse(src.AiringDate, null)))
				.ForMember(dest => dest.JapaneseTitle, opt => opt.MapFrom(src => src.JapaneseTitle))
				.ForMember(dest => dest.TitleSynonyms, opt => opt.MapFrom(src => src.TitleSynonyms))
				.ForMember(dest => dest.Season, opt => opt.MapFrom(src => src.Season.Name + " " + src.Season.Year))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
			
			CreateMap<Anime, AnimeTableEntryDto>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
				.ForMember(dest => dest.AiringFrom, opt => opt.MapFrom(src => System.DateTime.Parse(src.AiringDate, null)))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => malAnimeBaseUrl + src.MalId));
		}
	}
}