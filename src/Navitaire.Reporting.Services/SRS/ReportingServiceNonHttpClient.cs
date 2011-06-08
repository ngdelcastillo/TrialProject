using System;
using System.Net;

using Microsoft.SqlServer.ReportingServices.ReportService2005;

namespace Navitaire.Reporting.Services.SRS
{
	/// <summary>
	/// A proxy client class wrapper that includes cookie management for non-http clients.
	/// </summary>
	public class ReportingServiceNonHttpClient : ReportingService2005
	{
		private CookieContainer _cookies = new CookieContainer();

		/// <summary>
		/// Constructor that initializes client with the supplied credentials and loads the Web Service Url 
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="domain"></param>
        /// <param name="reportServiceUrl"></param>
		public ReportingServiceNonHttpClient( string userName, string password, string domain, string reportServiceUrl )
		{
			// Set the server URL
            base.Url = reportServiceUrl;

			// set the proxy client to use this user's creds
			base.Credentials = new NetworkCredential( userName, password, domain );

			base.LogonUser( userName, password, domain );
			
			base.Timeout = -1;
		}

		protected override WebRequest GetWebRequest( Uri uri )
		{
			HttpWebRequest req = ( HttpWebRequest ) base.GetWebRequest( uri );

			if ( _cookies != null )
			{
				req.CookieContainer = _cookies;
			}

			return req;
		}

		protected override WebResponse GetWebResponse( WebRequest request )
		{
			HttpWebResponse response = ( HttpWebResponse ) base.GetWebResponse( request );

			if ( response.Headers[ "Set-Cookie" ] != null )
			{
				_cookies.SetCookies( response.ResponseUri, response.Headers[ "Set-Cookie" ] );
			}

			return response;
		}
	}
}
