using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WatchTogether
{
	public class JavaScriptsHolder
	{
		private static readonly IDictionary<string, string> scriptsDictionary = new Dictionary<string, string>();

		private const string HelperJavaScriptIdentificator = "WatchTogether.Resources.JavaScripts.";
		private const string JavaScriptExt = ".js";

		/// <summary>
		/// Fills the scripts dictionary with values read from the assembly resources inside JavaScripts folder
		/// </summary>
		static JavaScriptsHolder()
		{
			// Get the  current execution assembly object
			var currentAssembly = Assembly.GetExecutingAssembly();
			// Get names of all the embedded resources in the assembly
			var resourceNames = currentAssembly.GetManifestResourceNames();

			// Loop through all the resources' names
			for (int i = 0; i < resourceNames.Length; i++)
			{
				// Get the i-th element from the array
				var resourceName = resourceNames[i];
				// If the current resourceName starts with HelperJavaScriptIdentificator then it's the
				// helper script we want to load
				if (resourceName.StartsWith(HelperJavaScriptIdentificator) == true &&
					resourceName.EndsWith(JavaScriptExt) == true)
				{
					// Get needed streams
					using (Stream stream = currentAssembly.GetManifestResourceStream(resourceName))
					using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
					{
						// Generate a key that users can use to obtain the required script code
						// Example:
						//		WatchTogether.Resources.JavaScripts.TestScript.js -> TestScript
						//
						var key = resourceName.Replace(HelperJavaScriptIdentificator, string.Empty).
							Replace(JavaScriptExt, string.Empty);
						// Read a scrip code
						var scriptCode = reader.ReadToEnd();

						// Add a new key-value pair to the scriptsDictionary object
						scriptsDictionary.Add(key, scriptCode);
					}
				}
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
