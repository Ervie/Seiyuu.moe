using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Enums;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	class SearchCriteriaProfile : Profile
	{
		public SearchCriteriaProfile()
		{
			CreateMap<SeasonSummarySearchCriteria, RoleSearchCriteria>()
				.ForMember(dest => dest.RoleTypeId, opt => opt.MapFrom(src => (src.MainRolesOnly ? RoleTypeDictionary.Main : RoleTypeDictionary.Supporting)));

			CreateMap<SeasonSummarySearchCriteria, AnimeSearchCriteria>()
				.ForMember(dest => dest.AnimeTypeId, opt => opt.Condition(src => src.TVSeriesOnly))
				.ForMember(dest => dest.AnimeTypeId, opt => opt.MapFrom(src => AnimeTypeDictionary.TV))
				.ForMember(dest => dest.SeasonId, opt => opt.MapFrom(src => src.Id));
		}
	}
}
