using System.Collections.Generic;

namespace WatchTogether.Browser.BrowserCommands
{
	interface IBrowserCommand
	{
		/// <summary>
		/// Initializes a browser command. Note that only System.Int32, System.Boolean and System.String types are allowed
		/// to be passed as elements of parameters array
		/// </summary>
		/// <param name="parameters">The input parameters needed for execution</param>
		void Initialize(params object[] parameters);

		/// <summary>
		/// Executes the main logic of a browser command
		/// </summary>
		void Execute();
	}
}
