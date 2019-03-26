using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;
using System;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	class SeiyuuProfile : Profile
	{
		private const string malPersonBaseUrl = "https://myanimelist.net/people/";

		public SeiyuuProfile()
		{
			CreateMap<Seiyuu, SeiyuuSearchEntryDto>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

			CreateMap<Seiyuu, SeiyuuCardDto>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
				.ForMember(dest => dest.JapaneseName, opt => opt.MapFrom(src => src.JapaneseName))
				.ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Birthday) ? DateTime.ParseExact(src.Birthday, "dd-MM-yyyy", null) : (DateTime?)null))
				.ForMember(dest => dest.About, opt => opt.MapFrom(src => src.About));

			CreateMap<Seiyuu, SeiyuuTableEntryDto>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => malPersonBaseUrl + src.MalId));
		}
	}
}
