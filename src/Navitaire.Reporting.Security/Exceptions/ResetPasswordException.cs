using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Security.Exceptions
{
	[Serializable]
	public class ResetPasswordException : Exception
	{
		public ResetPasswordException()
		{
		}

		public ResetPasswordException( string message )
			: base( message )
		{
		}

		public ResetPasswordException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

		protected ResetPasswordException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		public override string Message
		{
			get
			{
				return "The password has expired.";
			}
		}
	}
}
