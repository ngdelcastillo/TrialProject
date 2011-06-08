using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NReport : NCatalogItem
    {
        #region Constructor
        public NReport(string name, string path)
            : base(Constants.CatalogItemTypes.Report, Constants.CatalogItemTypeIcons.Report, name, path)
        { }
        #endregion
    }
}
