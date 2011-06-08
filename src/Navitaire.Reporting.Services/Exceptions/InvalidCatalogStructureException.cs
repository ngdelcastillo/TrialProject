using System;
using System.Runtime.Serialization;

namespace Navitaire.Reporting.Services.Exceptions
{
	[Serializable]
	public class InvalidCatalogStructureException : Exception
	{
		public InvalidCatalogStructureException()
		{
		}

		public InvalidCatalogStructureException( string message )
			: base( message )
		{
		}

		public InvalidCatalogStructureException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

		protected InvalidCatalogStructureException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		public override string Message
		{
			get
			{
				return string.Format( "Invalid Catalog Structure: {0}", base.Message );
			}
		}
	}
}
