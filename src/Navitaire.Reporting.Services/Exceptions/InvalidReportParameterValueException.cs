using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class InvalidReportParameterValueException : Exception
    {
        public InvalidReportParameterValueException()
        {
        }

        public InvalidReportParameterValueException( string message )
            : base( message )
        {
        }

        public InvalidReportParameterValueException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected InvalidReportParameterValueException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "There is a problem with your parameter setting: {0}", base.Message );
            }
        }
    }
}
