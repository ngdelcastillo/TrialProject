using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;
namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOConfiguredDataSourceCollection : ConfiguredDataSourceCollectionGen
    {
        #region Properties
        public BOConfiguredDataSource this[Guid configuredDataSourceId]
        {
            get
            {
                foreach (BOConfiguredDataSource configuredDataSource in this)
                {
                    if (configuredDataSource.ConfiguredDataSourceId.Equals(configuredDataSourceId))
                    {
                        return configuredDataSource;
                    }
                }

                return null;
            }
        }

        public BOConfiguredDataSource this[string catalogPath]
        {
            get
            {
                foreach (BOConfiguredDataSource configuredDataSource in this)
                {
                    if (configuredDataSource.CatalogPath.Equals(catalogPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return configuredDataSource;
                    }
                }

                return null;
            }
        }
        #endregion

        #region Methods
        public void Fill()
        {
            this.Clear();

            base.SelectConfiguredDataSourceList();
        }

        public bool Remove(Guid configuredDataSourceId)
        {
            BOConfiguredDataSource configuredDataSource = this[configuredDataSourceId];
            if (configuredDataSource != null)
            {
                return this.Remove(configuredDataSource);
            }

            return true;
        }

        public bool Remove(string catalogPath)
        {
            BOConfiguredDataSource configuredDataSource = this[catalogPath];
            if (configuredDataSource != null)
            {
                return this.Remove(configuredDataSource);
            }

            return true;
        }
        #endregion

    }
}
