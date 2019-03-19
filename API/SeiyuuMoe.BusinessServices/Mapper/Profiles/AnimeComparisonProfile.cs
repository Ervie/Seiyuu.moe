using AutoMapper;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	internal class AnimeComparisonProfile : Profile
	{
		public AnimeComparisonProfile()
		{
			CreateMap<AnimeComparisonEntry, AnimeComparisonEntryDto>()
				.ForMember(dest => dest.Seiyuu, opt => opt.MapFrom(src => src.Seiyuu))
				.ForMember(dest => dest.CharacterAnimePairs, opt => opt.MapFrom(src => src.CharacterAnimePairs));

			CreateMap<CharacterAnimePair, CharacterAnimePairDto>()
				.ForMember(dest => dest.Anime, opt => opt.MapFrom(src => src.Anime))
				.ForMember(dest => dest.Character, opt => opt.MapFrom(src => src.Character));
		}
	}
}
