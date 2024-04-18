using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace NewsAppExample.Helper
{
    public class ConfigReader
    {
        public static string getConnectionString ()
        {
            string connectionString = ReadConfig()["connectionString"].ToString();

            if (connectionString == null ) 
                throw new Exception("connectionstring could not be found in the config file");
            

            return connectionString;
        }

        public static string getJwtKey()
        {
            string jwtKey = ReadConfig()["jwtKey"].ToString();

            if (jwtKey == null)
                throw new Exception("jwtKey could not be found in the config file");


            return jwtKey;
        }


        private static JObject ReadConfig()
        {
            // Read the contents of the config file
            string configFilePath = "config.json";
            string jsonConfig = File.ReadAllText(configFilePath);

            // Parse the JSON content
            JObject configObject = JObject.Parse(jsonConfig);

            return configObject;
        }
    }
}
