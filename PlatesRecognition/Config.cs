using System;

namespace PlatesRecognition
{
    public class Config
    {

        public string Country { get; private set; }

        public string ConfigFile { get; set; }

        public string RuntimeDir { get; set; }
        public Config()
        {
            Country = "eu";

            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            ConfigFile = string.Concat(appPath, @"\lib\openalpr.conf");

            RuntimeDir = string.Concat(appPath, @"\lib\runtime_data");

        }
    }
}
