using System;
using System.Globalization;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NDatasourceReference
    {
        #region Properties
        public string Name { get; set; }
        public string DatasourcePath { get; set; }
        #endregion

        #region Constructor
        public NDatasourceReference(string name, string datasourcePath)
        {
            Name = name;
            DatasourcePath = datasourcePath;
        }
        #endregion

        #region Method
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Name, DatasourcePath);
        }
        #endregion
    }
}
