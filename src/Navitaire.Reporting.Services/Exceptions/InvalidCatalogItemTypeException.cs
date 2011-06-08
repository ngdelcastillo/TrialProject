using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
	[Serializable]
	public class InvalidCatalogItemTypeException : Exception
	{
		public InvalidCatalogItemTypeException()
		{
		}

		public InvalidCatalogItemTypeException( string message )
			: base( message )
		{
		}

		public InvalidCatalogItemTypeException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

		protected InvalidCatalogItemTypeException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		public override string Message
		{
			get
			{
				return string.Format( "Invalid Catalog Item Type: {0}", base.Message );
			}
		}
	}
}
