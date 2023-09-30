using Newtonsoft.Json;
using System;
using System.IO;

namespace BoxesProject.Conf
{
    public class Configurations
    {
        private static readonly string _programPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));// determine the path of the program's executable file or the root directory

        public static Configurations _instance;
        public static Configurations Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Configurations();
                return _instance;
            }
        }

        public ConfigData Data { get; set; }


        //load in private constructor
        private Configurations()
        {
            string fileName = "config.json";
            string configPath = Path.Combine(_programPath, fileName);
            string raw = File.ReadAllText(configPath);
            Data = JsonConvert.DeserializeObject<ConfigData>(raw);
        }

        //function that save the changes
        public void ChangeConfiguration()
        {
            string data = JsonConvert.SerializeObject(this.Data);
            File.WriteAllText(Path.Combine(_programPath, "config.json"), data);
        }

        public override string ToString()
        {
            return $"Expire Days: {Data.EXPIRE_DAYS}\n" +
                $"Max Boxes: {Data.MAX_BOXES}\n" +
                $"Min Boxes: {Data.MIN_BOXES}\n" +
                $"Deviation Percantage: {Data.DEVIATION_PERCANTANGE}";
        }
    }
}
