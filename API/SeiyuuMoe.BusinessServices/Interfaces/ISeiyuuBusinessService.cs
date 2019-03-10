﻿using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	public interface ISeiyuuBusinessService
	{
		Task<PagedResult<SeiyuuSearchEntryDto>> GetAsync(Query<SeiyuuSearchCriteria> query);
	}
}
