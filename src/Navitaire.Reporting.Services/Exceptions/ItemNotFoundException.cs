using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException()
        {
        }

        public ItemNotFoundException( string message )
            : base( message )
        {
        }

        public ItemNotFoundException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        protected ItemNotFoundException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public override string Message
        {
            get
            {
                return string.Format( "Cannot find the specified item: {0}", base.Message );
            }
        }
    }
}
