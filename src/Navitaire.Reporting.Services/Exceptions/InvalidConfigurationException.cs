using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException()
        {
        }

        public InvalidConfigurationException( string message )
            : base( message )
        {
        }

        public InvalidConfigurationException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected InvalidConfigurationException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "There is a problem with the configuration for this operation: {0}", base.Message );
            }
        }
    }
}
