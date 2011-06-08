using System;
using System.Net;
using System.Web;

using Microsoft.SqlServer.ReportingServices.ReportService2005;

using Navitaire.Reporting.Services.Exceptions;

namespace Navitaire.Reporting.Services.SRS
{
	/// <summary>
	/// A proxy client class wrapper that includes cookie management. This, in turn, supports the SRS Web Service's authentication
	/// ticket scheme for http clients such as web browsers. After instantiation, call the SRS LogonUser web method before making
	/// any other web service calls. SRS will generate an auth token in the form of a cookie and this class will manage it for you.
	/// </summary>
	public class ReportingServiceClient : ReportingService2005
	{
		private NetworkCredential _creds;
		private Cookie _authcookie;

		/// <summary>
		/// Constructor that initializes client with the supplied credentials and loads the Web Service Url 
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="domain"></param>
        /// <param name="reportServiceUrl"></param>
		public ReportingServiceClient( string userName, string password, string domain, string reportServiceUrl )
		{
			// Set the server URL
            base.Url = reportServiceUrl;

			// set the proxy client to use this user's creds
			base.Credentials = _creds = new NetworkCredential( userName, password, domain );

			base.AllowAutoRedirect = false;
			base.Timeout = -1;
		}

		/// <summary>
		/// Read Only.
		/// </summary>
		public NetworkCredential NewSkiesCredentials
		{
			get
			{
				return _creds;
			}
		}

		// This section of from a Microsoft sample implementation of forms auth on SRS
		// It essentially handles auth cookie management for the SRS web service
		protected override WebRequest GetWebRequest( Uri uri )
		{
			HttpWebRequest request;
			request = ( HttpWebRequest ) HttpWebRequest.Create( uri );
			// Create a cookie jar to hold the request cookie
			CookieContainer cookieJar = new CookieContainer();
			request.CookieContainer = cookieJar;
			Cookie authCookie = AuthCookie;
			// if the client already has an auth cookie
			// place it in the request's cookie container
			if ( authCookie != null )
				request.CookieContainer.Add( authCookie );
			request.Timeout = -1;
			request.Headers.Add( "Accept-Language",
			   HttpContext.Current.Request.Headers[ "Accept-Language" ] );
			return request;
		}

		protected override WebResponse GetWebResponse( WebRequest request )
		{
			WebResponse response = base.GetWebResponse( request );
			string cookieName = response.Headers[ "RSAuthenticationHeader" ];
			// If the response contains an auth header, store the cookie
			if ( cookieName != null )
			{
				Utilities.CustomAuthCookieName = cookieName;
				HttpWebResponse webResponse = ( HttpWebResponse ) response;
				Cookie authCookie = webResponse.Cookies[ cookieName ];
				// If the auth cookie is null, throw an exception
				if ( authCookie == null )
				{
					throw new InvalidAuthorizationException(
					   "Authorization ticket not received by LogonUser" );
				}
				// otherwise save it for this request
				AuthCookie = authCookie;
				// and send it to the client
				Utilities.RelayCookieToClient( authCookie );
			}
			return response;
		}

		public Cookie AuthCookie
		{
			get
			{
				if ( _authcookie == null )
					_authcookie =
					Utilities.TranslateCookie(
					   HttpContext.Current.Request.Cookies[ Utilities.CustomAuthCookieName ] );

				return _authcookie;
			}
			set
			{
				_authcookie = value;
			}
		}
	}


	// This section of from a Microsoft sample implementation of forms auth on SRS
	// It is used to help handle auth cookie management for the SRS web service
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses" )]
	internal sealed class Utilities
	{
		internal static string CustomAuthCookieName
		{
			get
			{
				lock ( cookieNameLockRoot )
				{
					return cookieName;
				}
			}
			set
			{
				lock ( cookieNameLockRoot )
				{
					cookieName = value;
				}
			}
		}

		private static string cookieName;
		private static object cookieNameLockRoot = new object();

		private static HttpCookie TranslateCookie( Cookie netCookie )
		{
			if ( netCookie == null )
				return null;
			HttpCookie webCookie = new HttpCookie( netCookie.Name, netCookie.Value );
			// Add domain only if it is dotted - IE doesn't send back the cookie 
			// if we set the domain otherwise
			if ( netCookie.Domain.IndexOf( '.' ) != -1 )
				webCookie.Domain = netCookie.Domain;
			webCookie.Expires = netCookie.Expires;
			webCookie.Path = netCookie.Path;
			webCookie.Secure = netCookie.Secure;
			return webCookie;
		}

		internal static Cookie TranslateCookie( HttpCookie webCookie )
		{
			if ( webCookie == null )
				return null;
			Cookie netCookie = new Cookie( webCookie.Name, webCookie.Value );
			if ( webCookie.Domain == null )
				netCookie.Domain =
				   HttpContext.Current.Request.ServerVariables[ "SERVER_NAME" ];
			netCookie.Expires = webCookie.Expires;
			netCookie.Path = webCookie.Path;
			netCookie.Secure = webCookie.Secure;
			return netCookie;
		}

		internal static void RelayCookieToClient( Cookie cookie )
		{
			// add the cookie if not already in there
			if ( HttpContext.Current.Response.Cookies[ cookie.Name ] == null )
			{
				HttpContext.Current.Response.Cookies.Remove( cookie.Name );
			}

			HttpContext.Current.Response.SetCookie( TranslateCookie( cookie ) );
		}
	}
}
