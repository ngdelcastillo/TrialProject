using System;
using System.Globalization;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NDatasource : NCatalogItem
    {
        #region Properties
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Password { get; set; }
        public string Provider { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }

        #endregion

        #region Constructors
        public NDatasource(string name, string path)
            : base(Constants.CatalogItemTypes.DataSource, Constants.CatalogItemTypeIcons.Datasource, name, path)
        { }

        public NDatasource(string name, string path, string provider, string serverName, string databaseName, string userName, string password)
            : base(Constants.CatalogItemTypes.DataSource, Constants.CatalogItemTypeIcons.Datasource, name, path)
        {
            Provider = provider;
            ServerName = serverName;
            DatabaseName = databaseName;
            UserName = userName;
            Password = password;
            ConnectionString = string.Format(CultureInfo.InvariantCulture, @"data source={0};initial catalog={1}", serverName, databaseName);
        }
        #endregion
    }
}
