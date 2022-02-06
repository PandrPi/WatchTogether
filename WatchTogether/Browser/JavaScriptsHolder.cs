using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WatchTogether.Browser
{
    public class JavaScriptsHolder
    {
        private static readonly IDictionary<string, string> scriptsDictionary = new Dictionary<string, string>();

        private const string HelperJavaScriptIdentificator = "WatchTogether.Resources.JavaScripts.";
        private const string JavaScriptExt = ".js";

        /// <summary>
        /// Fills the scripts dictionary with values read from the assembly resources inside JavaScripts folder
        /// </summary>
        public static void Initialize()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var resourceNames = currentAssembly.GetManifestResourceNames();

            LoadScriptsFromResources(currentAssembly, resourceNames);
        }

        private static void LoadScriptsFromResources(Assembly currentAssembly, string[] resourceNames)
        {
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith(HelperJavaScriptIdentificator) == true &&
                    resourceName.EndsWith(JavaScriptExt) == true)
                {
                    ReadScriptFromResorcesByName(currentAssembly, resourceName);
                }
            }
        }

        private static void ReadScriptFromResorcesByName(Assembly currentAssembly, string resourceName)
        {
            using (Stream stream = currentAssembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                var key = resourceName.Replace(HelperJavaScriptIdentificator, string.Empty).
                    Replace(JavaScriptExt, string.Empty);
                
                var scriptCode = reader.ReadToEnd();

                scriptsDictionary.Add(key, scriptCode);
            }
        }

        /// <summary>
        /// Gets a script code from an internal dictionary by the specified key
        /// </summary>
        /// <param name="key">The key to the desired script code</param>
        /// <returns></returns>
        public static string GetScriptCodeByKey(string key) => scriptsDictionary[key];
    }
}
