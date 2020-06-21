﻿using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public interface IAnimeTypeRepository
	{
		Task<AnimeType> GetByName(string typeName);
	}
}