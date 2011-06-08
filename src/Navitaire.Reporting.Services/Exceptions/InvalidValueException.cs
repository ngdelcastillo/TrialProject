using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class InvalidValueException : Exception
    {
        public InvalidValueException()
        {
        }

        public InvalidValueException( string message )
            : base( message )
        {
        }

        public InvalidValueException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected InvalidValueException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "There was a problem with the specified value: {0}", base.Message );
            }
        }
    }
}
