using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeiyuuMoe.Services
{
	public class JSONSerializerService
	{
		private readonly string resourcesPath;

		public IConfiguration Configuration { get; set; }

		public JSONSerializerService(IConfiguration configuration)
		{
			Configuration = configuration;

			resourcesPath = AppContext.BaseDirectory + configuration["configValues:resourcePath"];
		}

		public List<T> LoadFromFile<T>(string fileName)
		{
			var text = File.ReadAllText(resourcesPath + fileName, Encoding.UTF8);

			var deserializedList = JsonConvert.DeserializeObject<List<T>>(text);

			return deserializedList ?? new List<T>();
		}

		public void SaveToFile<T>(string fileName, List<T> dataToSave)
		{
			var json = JsonConvert.SerializeObject(dataToSave);
			File.WriteAllText(resourcesPath + fileName, json, Encoding.UTF8);
		}
	}
}