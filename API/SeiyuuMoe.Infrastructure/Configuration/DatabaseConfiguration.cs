namespace SeiyuuMoe.Infrastructure.Configuration
{
	public class DatabaseConfiguration
	{
		public string Host { get; set; }

		public int Port { get; set; }

		public string User { get; set; }

		public string Password { get; set; }

		public string Database { get; set; }

		public string ToConnectionString
			=> $"Server={Host};Port={Port};Database={Database};Uid={User};Pwd={Password};Connect Timeout=600;";
	}
}