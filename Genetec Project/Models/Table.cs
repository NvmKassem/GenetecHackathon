using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Genetec_Project.Models
{
    class Table
    {
        public static string[] table = new string[] {"003VLH", "025WFD", "027SSD", "034XMW", "065TZN", "103EEN", "106XBJ", "170YYT", "186ZMX", "204PYE", "217TAP", "218ZLZ", "220WNZ", "241ZCK", "250ZHW", "327WST", "330XNV", "347WRK", "347WVL", "498YRH", "533SLM", "569SRT", "737XPN", "B51AGZ", "E97BAC"};

        public static string filepath = @"file.json";

        public static async Task readFile() {
            using (StreamReader r = new StreamReader(filepath)) {
                string json = r.ReadToEnd();
                string[] items = JsonConvert.DeserializeObject<string[]>(json);
                Console.WriteLine("[{0}]", string.Join(", ", items));
                table = items;
            }
        }

        public static void updateFile(string[] list) {
            table = list;
            var lst = "[" + string.Join(", ", "\""+list+ "\"") + "]";
            File.WriteAllText(filepath, lst);
        }
    }
}
