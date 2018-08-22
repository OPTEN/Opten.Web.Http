using Opten.Common.Extensions;
using System;

namespace Opten.Web.Http
{
	/// <summary>
	/// The Web API Url Helper.
	/// </summary>
	public static class UrlHelper
	{

		/// <summary>
		/// Gets the current url.
		/// </summary>
		/// <param name="urlHelper">The URL helper.</param>
		/// <param name="withQuery">if set to <c>true</c> [with query].</param>
		/// <param name="withDomain">if set to <c>true</c> [with domain].</param>
		/// <returns></returns>
		public static string Current(this System.Web.Http.Routing.UrlHelper urlHelper, bool withQuery = true, bool withDomain = true)
		{
			return urlHelper.Current().GetUrl(withQuery, withDomain);
		}

		/// <summary>
		/// Gets the current url.
		/// </summary>
		/// <param name="urlHelper">The URL helper.</param>
		/// <returns></returns>
		public static Uri Current(this System.Web.Http.Routing.UrlHelper urlHelper)
		{
			return urlHelper.Request.RequestUri;
		}

		/// <summary>
		/// Gets the base current url.
		/// </summary>
		/// <param name="urlHelper">The URL helper.</param>
		/// <returns></returns>
		public static string BaseUrl(this System.Web.Http.Routing.UrlHelper urlHelper)
		{
			return urlHelper.Request.RequestUri.GetBaseUrl();
		}

	}
}