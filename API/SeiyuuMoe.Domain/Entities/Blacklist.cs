using System;
using System.ComponentModel.DataAnnotations;

namespace SeiyuuMoe.Domain.Entities
{
	public class Blacklist
	{
		[Key]
		public Guid Id { get; set; }
		public long MalId { get; set; }
		public string EntityType { get; set; }
		public string Reason { get; set; }
	}
}