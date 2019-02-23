using AutoMapper;
using System.Collections.Generic;

namespace SeiyuuMoe.Services.Mapper
{
	internal class AutoMapperConfiguration : MapperConfiguration
	{
		public AutoMapperConfiguration(IEnumerable<Profile> profiles)
			   : base(cfg =>
			   {
				   foreach (var profile in profiles)
				   {
					   cfg.AddProfile(profile);
				   }
			   })
		{
		}
	}
}
