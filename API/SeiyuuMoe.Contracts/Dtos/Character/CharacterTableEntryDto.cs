using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Contracts.Dtos
{
	public class CharacterTableEntryDto
	{
		public string Name { get; set; }

		public string ImageUrl { get; set; }

		public string Url { get; set; }

		public long MalId { get; set; }
	}
}
