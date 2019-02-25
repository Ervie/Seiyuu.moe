using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.ServBusinessServicesices.Mapper.Profiles
{
	internal class AnimeProfile : Profile
	{
		public AnimeProfile()
		{
			CreateMap<Anime, AnimeDto>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.AiringFrom, opt => opt.MapFrom(src => src.AiringDate))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

			CreateMap<Anime, AnimeAiringDto>()
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.AiringFrom, opt => opt.MapFrom(src => src.AiringDate));
		}
	}
}