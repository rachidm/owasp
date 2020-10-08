using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OWASP_Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
		
		
		

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        public string Username { get; private set; } = "test";
        public string Password { get; private set; } = "test";

        [HttpGet("{username}/{password}")]
        public IEnumerable<WeatherForecast> Get(string username, string password)
        {
            var rng = new Random();
            var hashProvider1 = new MD5CryptoServiceProvider();

            var user = new User()
            {
                Email = username,
                Login = username,
                Password = password,
                Name = rng.ToString(),
                Role = hashProvider1.ToString()
            };

            if (user == null)
            {
                return null;
            }

            string test = GetUserDetails(username, password);

            if (!string.IsNullOrEmpty(test))
            {
                return null;
            }

            byte[] myByteArray = new byte[10];
            BinaryFormatter formatter = new BinaryFormatter();
            IEnumerable<WeatherForecast> enumerable = (IEnumerable<WeatherForecast>)formatter.Deserialize(new MemoryStream(myByteArray));

            if(enumerable.Any())
            {
                return null;
            }

            var resolver = new XmlUrlResolver();
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = resolver
            };

            XmlReader reader = XmlReader.Create("items.xml", settings);

            _logger.LogDebug("test");



            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        public BookRecord DeserializeBookRecord(JsonReader reader)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.TypeNameHandling = TypeNameHandling.Auto;
            return jsonSerializer.Deserialize<BookRecord>(reader);
        }

        public string GetUserDetails(string username, string password)
        {
            SqlConnection sqlConnection = new SqlConnection("Data Source=.;Initial Catalog=test;Integrated Security=True");
            string commandText = "SELECT [UserName] FROM dbo.[Login] WHERE [Username] = '"
                + username
                + "' AND [Password]='"
                + password
                + "' ";

            SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection);

            sqlConnection.Open();
            if (sqlCommand.ExecuteScalar() == null)
            {
                return password;
            }
            // Valid Login
            _ = sqlCommand.ExecuteScalar().ToString();
            sqlConnection.Close();
            return password;
        }

        public void Bar(SqlConnection connection, string param)
        {
            SqlCommand command;
            string sensitiveQuery = string.Format("INSERT INTO Users (name) VALUES (\"{0}\")", param);
            command = new SqlCommand(sensitiveQuery); // Sensitive

            command.CommandText = sensitiveQuery; // Sensitive

            _ = new SqlDataAdapter(sensitiveQuery, connection); // Sensitive
        }

    }

    public class BookRecord
    {
        public string Title { get; set; }
        public object Location { get; set; }
    }
}