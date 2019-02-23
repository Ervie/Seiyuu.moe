using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Repositories.Models
{
	public class SortExpression
	{
		public string Column { get; set; }
		public bool Desc { get; set; }

		public SortExpression(string column, bool desc)
		{
			this.Column = column;
			this.Desc = desc;
		}
	}
}
