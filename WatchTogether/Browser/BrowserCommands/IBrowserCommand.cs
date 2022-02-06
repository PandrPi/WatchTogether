using System.Collections.Generic;

namespace WatchTogether.Browser.BrowserCommands
{
	interface IBrowserCommand
	{
		/// <summary>
		/// Initializes a browser command
		/// </summary>
		/// <param name="parameters">The input parameters needed for execution</param>
		void Initialize(params object[] parameters);

		/// <summary>
		/// Executes the main logic of a browser command
		/// </summary>
		object Execute(SimpleTCP.Message message);
	}
}
