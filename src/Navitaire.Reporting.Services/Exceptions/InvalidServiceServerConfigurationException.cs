using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class InvalidServiceServerConfigurationException : Exception
    {
        public InvalidServiceServerConfigurationException()
        {
        }

        public InvalidServiceServerConfigurationException( string message )
            : base( message )
        {
        }

        public InvalidServiceServerConfigurationException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected InvalidServiceServerConfigurationException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "The service is not configured to run on this computer.: {0}", base.Message );
            }
        }
    }
}
