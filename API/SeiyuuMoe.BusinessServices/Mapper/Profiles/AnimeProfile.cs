using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.ServBusinessServicesices.Mapper.Profiles
{
	internal class AnimeProfile : Profile
	{
		public AnimeProfile()
		{
			CreateMap<Anime, AnimeSearchEntryDto>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

			CreateMap<Anime, AnimeAiringDto>()
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.AiringFrom, opt => opt.MapFrom(src => src.AiringDate));

			CreateMap<Anime, AnimeCardDto>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.About, opt => opt.MapFrom(src => src.About))
				.ForMember(dest => dest.AiringDate, opt => opt.MapFrom(src => src.AiringDate.Substring(1, 10)))
				.ForMember(dest => dest.JapaneseTitle, opt => opt.MapFrom(src => src.JapaneseTitle))
				.ForMember(dest => dest.TitleSynonyms, opt => opt.MapFrom(src => src.TitleSynonyms))
				.ForMember(dest => dest.Season, opt => opt.MapFrom(src => src.Season.Name + " " + src.Season.Year))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
		}
	}
}