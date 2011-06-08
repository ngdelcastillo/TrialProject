using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
    [Serializable]
    public class ItemAlreadyExistsException : Exception
    {
		public ItemAlreadyExistsException()
		{
		}

		public ItemAlreadyExistsException( string message )
			: base( message )
		{
		}

		public ItemAlreadyExistsException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

        protected ItemAlreadyExistsException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		public override string Message
		{
			get
			{
				return string.Format( "This item already exists: {0}", base.Message );
			}
		}
    }
}
