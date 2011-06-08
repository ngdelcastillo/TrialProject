using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class ReportExecutionFailedException : Exception
    {
        public ReportExecutionFailedException()
        {
        }

        public ReportExecutionFailedException( string message )
            : base( message )
        {
        }

        public ReportExecutionFailedException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected ReportExecutionFailedException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "An error occurred while running the report: {0}", base.Message );
            }
        }
    }
}