using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Security.Exceptions
{
	[Serializable]
	public class PasswordsMustMatchException : Exception
	{
		public PasswordsMustMatchException()
		{
		}

		public PasswordsMustMatchException( string message )
			: base( message )
		{
		}

		public PasswordsMustMatchException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

		protected PasswordsMustMatchException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		public override string Message
		{
			get
			{
				return "The passwords do not match.";
			}
		}
	}
}
