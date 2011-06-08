using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NFolder : NCatalogItem
    {
        #region Properties
        public NCatalogItemCollection ChildCatalogItems { get { return new NCatalogItemCollection(); } }
        #endregion

        #region Constructor
        public NFolder(string name, string path)
            : base(Constants.CatalogItemTypes.Folder, Constants.CatalogItemTypeIcons.Folder, name, path)
        { }

        #endregion
    }
}
