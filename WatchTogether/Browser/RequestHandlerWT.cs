using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;

namespace WatchTogether.Browser
{
	class RequestHandlerWT : RequestHandler
	{
		private static readonly Regex videoFrameRegex = 
			new Regex("https://videoframe.space/[a-zA-Z]+/[a-zA-Z0-9]+/iframe", RegexOptions.Compiled);

		/// <summary>
		/// Called before browser navigation. If the navigation is allowed CefSharp.IWebBrowser.FrameLoadStart
		///  and CefSharp.IWebBrowser.FrameLoadEnd will be called. If the navigation is canceled
		///  CefSharp.IWebBrowser.LoadError will be called with an ErrorCode value of CefSharp.CefErrorCode.Aborted.
		/// </summary>
		/// <returns>Return true to cancel the navigation or false to allow the navigation to proceed</returns>
		protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
		{
			if (videoFrameRegex.IsMatch(request.Url) && request.ResourceType == ResourceType.SubFrame)
			{
				// When the browser sends a request to load a document from videoframe domain we can stop the page
				// loading and instantly redirect the browser two the request.Url because the document we want to reach
				// is placed exactly there
				browser.StopLoad();
				chromiumWebBrowser.Load(request.Url);
			}

			return false;
		}
	}
}
