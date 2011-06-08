using System;
using System.Collections.ObjectModel;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NCatalogItemCollection : Collection<NCatalogItem>
    {
        public NCatalogItem this[string itemPath]
        {
            get
            {
                foreach (NCatalogItem item in this)
                {
                    if (item.Path.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
                    { 
                        return item; 
                    }
                }
                return null;
            }
        }
    }
}
