using System;
using System.Collections.Specialized;
using System.Net;

using Microsoft.SqlServer.ReportingServices.ReportExecutionService;

namespace Navitaire.Reporting.Services.SRS
{
    /// <summary>
    /// A proxy client class wrapper that includes cookie management for non-http clients. This, in turn, supports the 
    /// SRS Web Service's authentication ticket scheme for non-http clients. This class's constructors call the SRS LogonUser 
    /// web method and will manage the SRS auth token for you. This class is intended for use by command line utilities, 
    /// winforms, and the like that do not have a built in HttpContext. 
    /// DO NOT USE THIS CLASS FOR BROWSER CLIENTS - it in inherently less secure!
    /// </summary>
    public class ExecutionServiceClient : ReportExecutionService
    {
        private CookieContainer _cookies = new CookieContainer();
        private StringCollection _languages = new StringCollection();

        /// <summary>
        /// Constructor that initializes client with the supplied credentials and loads the Web Service Url
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="executionServiceUrl"></param>
        public ExecutionServiceClient( string userName, string password, string domain, string executionServiceUrl )
        {
            // Set the server URL
            base.Url = executionServiceUrl;

            // set the proxy client to use this user's creds
            base.Credentials = new NetworkCredential( userName, password, domain );

            // set timeout to unlimited
            base.Timeout = -1;

            // set default language to "en-US"
            _languages.Add( "en-US" );
        }

        /// <summary>
        /// Gets or Sets the culture codes representing the ACCEPT-LANGUAGES header in the request.
        /// This property is used internally to build the request header for SRS so it gets a response 
        /// based on the languages set in this property. This has a default index [0] of "en-US".
        /// </summary>
        public StringCollection Languages
        {
            get
            {
                return _languages;
            }
            //set
            //{
            //    _languages = value;
            //}
        }

        protected override WebRequest GetWebRequest( Uri uri )
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest( uri );
            if ( _cookies != null )
            {
                req.CookieContainer = _cookies;
            }

            // add the languages to the header
            foreach ( String cultureCode in _languages )
            {
                req.Headers.Add( HttpRequestHeader.AcceptLanguage, cultureCode );
            }

            return req;
        }

        protected override WebResponse GetWebResponse( WebRequest request )
        {
            try
            {
                HttpWebResponse rep = (HttpWebResponse)base.GetWebResponse( request );
                if ( rep.Headers[ "Set-Cookie" ] != null )
                {
                    _cookies.SetCookies( rep.ResponseUri, rep.Headers[ "Set-Cookie" ] );
                }
                return rep;
            }
            catch// ( Exception ex )
            {
                //string message = ex.Message;
                return null;
            }
        }
    }
}
