using AutoMapper;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos.Other;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	internal class SeiyuuComparisonProfile : Profile
	{
		public SeiyuuComparisonProfile()
		{
			CreateMap<SeiyuuComparisonEntry, SeiyuuComparisonEntryDto>()
				.ForMember(dest => dest.Anime, opt => opt.MapFrom(src => src.Anime))
				.ForMember(dest => dest.SeiyuuCharacters, opt => opt.MapFrom(src => src.SeiyuuCharacters));

			CreateMap<SeiyuuComparisonSubEntry, SeiyuuComparisonSubEntryDto>()
				.ForMember(dest => dest.Seiyuu, opt => opt.MapFrom(src => src.Seiyuu))
				.ForMember(dest => dest.Characters, opt => opt.MapFrom(src => src.Characters));
		}
	}
}