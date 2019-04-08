using AutoMapper;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.BusinessServices
{
	class SeasonBusinessService: ISeasonBusinessService
	{
		private readonly IMapper mapper;
		private readonly ISeasonRepository seasonRepository;

		public SeasonBusinessService(
			IMapper mapper,
			ISeasonRepository seasonRepository
		)
		{
			this.mapper = mapper;
			this.seasonRepository = seasonRepository;
		}
	}
}
