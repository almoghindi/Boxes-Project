using BoxesProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BoxesProject.DAL
{
    public class LoadAndSaveJson
    {
        private static readonly string _programPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));// determine the path of the program's executable file or the root directory


        //load all boxes
        public static void LoadData()
        {
            try
            {
                string data = File.ReadAllText(Path.Combine(_programPath, "Boxes.json"));
                TreeManager.root = JsonConvert.DeserializeObject<BaseTree>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //save all boxes
        public static void SaveData()
        {
            try
            {
                string data = JsonConvert.SerializeObject(TreeManager.root);
                File.WriteAllText(Path.Combine(_programPath, "Boxes.json"), data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
