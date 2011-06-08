using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Security.Exceptions
{
    [Serializable]
    public class LogonException : Exception
    {
        public LogonException()
        {
        }

        public LogonException( string message )
            : base( message )
        {
        }

        public LogonException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected LogonException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "There was a problem logging on: {0}", base.Message );
            }
        }
    }
}
