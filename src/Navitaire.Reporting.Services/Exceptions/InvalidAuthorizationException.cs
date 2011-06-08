using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
	[Serializable]
	public class InvalidAuthorizationException : Exception
	{
		public InvalidAuthorizationException()
		{
		}

		public InvalidAuthorizationException( string message )
			: base( message )
		{
		}

		public InvalidAuthorizationException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

		protected InvalidAuthorizationException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		public override string Message
		{
			get
			{
				return string.Format( "Invalid Authorization Exception: {0}", base.Message );
			}
		}
	}
}
