using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOConfigurationCollection : ConfigurationCollectionGen
    {
        public BOConfiguration this[string propertyName]
        {
            get
            {
                foreach (BOConfiguration configuration in this)
                {
                    if (configuration.PropertyName.Equals(propertyName))
                    {
                        return configuration;
                    }
                }

                return null;
            }
        }

        public void Fill()
        {
            this.Clear();

            base.SelectConfigurationList();
        }
    }
}
