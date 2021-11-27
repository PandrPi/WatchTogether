using CefSharp;
using CefSharp.Wpf;
using System.Diagnostics;

namespace WatchTogether.Browser
{
	public class BrowserSettings
	{
		private CefSettings settings { get; set; }

		private static readonly string FlashPlayerPath = @"C:\Users\Andrushko\Desktop\pepflashplayer.dll";
		private static readonly string FlashPlayerVersion = FileVersionInfo.GetVersionInfo(FlashPlayerPath).FileVersion;

		public BrowserSettings()
		{
			settings = new CefSettings();
		}

		/// <summary>
		/// Initializes CefSettings and CommandLine arguments for the browser
		/// </summary>
		public void Initialize()
		{
			// Set all the needed command line arguments
			settings.CefCommandLineArgs["enable-system-flash"] = "1";
			settings.CefCommandLineArgs["ppapi-flash-path"] = FlashPlayerPath;
			settings.CefCommandLineArgs["ppapi-flash-version"] = FlashPlayerVersion;
			settings.CefCommandLineArgs.Add("plugin-policy", "allow");
			settings.CefCommandLineArgs.Add("allow-outdated-plugins", "allow");

			// Initialize Cef with the created settings
			Cef.Initialize(settings);

			//var contx = Cef.GetGlobalRequestContext();
			//Cef.UIThreadTaskFactory.StartNew(delegate
			//{
			//	contx.SetPreference("profile.default_content_setting_values.plugins", 1, out string err);
			//	Console.WriteLine(err);
			//});
		}
	}
}
