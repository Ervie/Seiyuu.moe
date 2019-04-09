using AutoMapper;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos.Season;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	internal class SeasonProfile : Profile
	{
		public SeasonProfile()
		{
			CreateMap<SeasonSummaryEntry, SeasonSummaryEntryDto>()
				.ForMember(dest => dest.Seiyuu, opt => opt.MapFrom(src => src.Seiyuu))
				.ForMember(dest => dest.AnimeCharacterPairs, opt => opt.MapFrom(src => src.AnimeCharacterPairs));
		}
	}
}