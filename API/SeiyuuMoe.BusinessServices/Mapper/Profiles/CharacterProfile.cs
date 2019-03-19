using AutoMapper;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.BusinessServices.Mapper.Profiles
{
	internal class CharacterProfile : Profile
	{
		private const string malCharacterBaseUrl = "https://myanimelist.net/character/";

		public CharacterProfile()
		{
			CreateMap<Character, CharacterTableEntryDto>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.MalId, opt => opt.MapFrom(src => src.MalId))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => malCharacterBaseUrl + src.MalId));
		}
	}
}