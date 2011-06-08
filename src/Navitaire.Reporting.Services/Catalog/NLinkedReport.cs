using System;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NLinkedReport : NCatalogItem
    {
        #region Constructor
        public NLinkedReport(string name, string path)
            : base(Constants.CatalogItemTypes.LinkedReport, Constants.CatalogItemTypeIcons.LinkedReport, name, path)
        { }
        #endregion
    }
}
