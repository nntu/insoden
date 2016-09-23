using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace insoden
{
    public class AppSettings<T> where T : new()
    {
        
        private const string DEFAULT_FILENAME = "settings.json";

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            //  File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));

            var output = JsonConvert.SerializeObject(this, Formatting.Indented, new StringEnumConverter { CamelCaseText = true });


            File.WriteAllText(fileName, output);
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            // File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));

            var output = JsonConvert.SerializeObject(pSettings, Formatting.Indented, new StringEnumConverter { CamelCaseText = true });


            File.WriteAllText(fileName, output);
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            var t = new T();
            if (File.Exists(fileName))
            {
                var input = File.ReadAllText(fileName);
                JsonConvert.PopulateObject(input, t, settings);
            }
            else
            {
                Save(t, fileName);
            }
            return t;
        }
    }
}
