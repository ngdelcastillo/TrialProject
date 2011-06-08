using System;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOConfiguredDataSource : ConfiguredDataSourceMessage
    {
        #region Constructors

        public BOConfiguredDataSource()
            : base()
        {
        }

        /// <summary>
        /// This constructor is used by Collections, it is not intended to be publicly consumable
        /// </summary>
        /// <param name="dr">The DataReader used to fill the underlying Message type</param>
        internal BOConfiguredDataSource(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructors

        #region Methods
        public void Delete()
        {
            base.Delete(ConfiguredDataSourceId);
        }

        public void Save(string authenticatedUser)
        {
            if (this.ConfiguredDataSourceId == Guid.Empty)
            {
                base.Insert(CatalogPath, ConnectionStringValue, ConnectionStringType, Credentials, authenticatedUser);
            }
            else
            {
                base.Update(ConfiguredDataSourceId, CatalogPath, ConnectionStringValue, ConnectionStringType, Credentials, authenticatedUser);
            }
        }
        #endregion
    }
}
