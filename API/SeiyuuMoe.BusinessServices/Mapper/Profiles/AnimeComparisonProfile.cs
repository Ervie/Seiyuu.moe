using AutoMapper;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	internal class AnimeComparisonProfile : Profile
	{
		public AnimeComparisonProfile()
		{
			CreateMap<AnimeComparisonEntry, AnimeComparisonEntryDto>()
				.ForMember(dest => dest.Seiyuu, opt => opt.MapFrom(src => src.Seiyuu))
				.ForMember(dest => dest.AnimeCharacters, opt => opt.MapFrom(src => src.AnimeCharacters));

			CreateMap<AnimeComparisonSubEntry, AnimeComparisonSubEntryDto>()
				.ForMember(dest => dest.Anime, opt => opt.MapFrom(src => src.Anime))
				.ForMember(dest => dest.Characters, opt => opt.MapFrom(src => src.Characters));
		}
	}
}
