using System;
using System.Collections.ObjectModel;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NDatasourceReferenceCollection : Collection<NDatasourceReference>
    {
        public NDatasourceReference this[string datasourcePath]
        {
            get
            {
                foreach (NDatasourceReference datasourceRef in this)
                {
                    if (datasourceRef.DatasourcePath.Equals(datasourcePath, StringComparison.OrdinalIgnoreCase))
                    {
                        return datasourceRef;
                    }                    
                }
                return null;
            }
        }
    }
}
