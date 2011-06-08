using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class DiagnosisException : Exception
    {
        public DiagnosisException()
        {
        }

        public DiagnosisException( string message )
            : base( message )
        {
        }

        public DiagnosisException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected DiagnosisException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "Diagnosis: {0}", base.Message );
            }
        }
    }
}