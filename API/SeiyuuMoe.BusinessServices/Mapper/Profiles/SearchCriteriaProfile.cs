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
				.ForMember(dest => dest.RoleTypeId, opt => opt.MapFrom(src => (src.MainRolesOnly ? (long) RoleTypeDictionary.Main : (long) RoleTypeDictionary.Supporting)))
				.ForMember(dest => dest.AnimeId, opt => opt.MapFrom(src => src.AnimeId));

			CreateMap<SeasonSummarySearchCriteria, AnimeSearchCriteria>()
				.ForMember(dest => dest.AnimeTypeId, opt => opt.MapFrom(src => (src.TVSeriesOnly ? (long) AnimeTypeDictionary.TV : (long) AnimeTypeDictionary.AllTypes)))
				.ForMember(dest => dest.SeasonId, opt => opt.MapFrom(src => src.Id));

			CreateMap<SeasonSummarySearchCriteria, SeasonSearchCriteria>()
				.ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Season));
		}
	}
}
