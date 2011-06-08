using System;
using System.Globalization;
using Microsoft.SqlServer.ReportingServices.ReportService2005;

namespace Navitaire.Reporting.Services.Catalog
{
    public abstract class NCatalogItem
    {
        #region Properties
        public string ImageUrlSuffix { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public NReportParameterCollection ReportParameters { get; set; }
        #endregion

        #region Constructor
        protected NCatalogItem(string itemType, string imageUrlSuffix, string name, string path)
        {
            ItemType = itemType;
            ImageUrlSuffix = imageUrlSuffix;
            Name = name;
            Path = path;
        }
        #endregion

        #region Methods
        public static NCatalogItem CreateNRCatalogItem(CatalogItem catalogItem)
        {
            #region Validate Parameters
            if (catalogItem == null)
            {
                throw new ArgumentNullException("ci");
            }
            #endregion ValidateParameters

            NCatalogItem navitaireCatalogItem = null;
            string parentPath = catalogItem.Path.Substring(0, catalogItem.Path.LastIndexOf('/'));
            switch (catalogItem.Type)
            {
                case ItemTypeEnum.DataSource:
                    navitaireCatalogItem = new NDatasource(catalogItem.Name, catalogItem.Path);
                    break;
                case ItemTypeEnum.Folder:
                    navitaireCatalogItem = new NFolder(catalogItem.Name, catalogItem.Path);
                    break;
                case ItemTypeEnum.LinkedReport:
                    navitaireCatalogItem = new NLinkedReport(catalogItem.Name, catalogItem.Path);
                    break;
                case ItemTypeEnum.Report:
                    navitaireCatalogItem = new NReport(catalogItem.Name, catalogItem.Path);
                    break;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an unsupported NRItemType", catalogItem.Type), "ci"); //Noel: what is ci?
            }
            return navitaireCatalogItem;
        }

        public static string FindNameFromSignature(string signature)
        {
            if (string.IsNullOrEmpty(signature))
            {
                return string.Empty;
            }
            else
            {
                return signature.Substring(signature.LastIndexOf("/") + 1);
            }
        }

        public static string FindPathFromSignature(string signature)
        {
            if (string.IsNullOrEmpty(signature))
            {
                return string.Empty;
            }
            else
            {
                int startIndex = signature.IndexOf(":")+1;
                return signature.Substring(startIndex);
            }
        }

        public static string FindFolderPathFromSignature(string signature, bool trimLastSlash)
        {
            if (string.IsNullOrEmpty(signature))
            {
                return string.Empty;
            }
            else
            {
                int startIndex = signature.IndexOf(":") + 1;
                int endIndex = signature.LastIndexOf("/") + 1;

                string folderPath = signature.Substring(startIndex, endIndex - startIndex);
                if (trimLastSlash)
                {
                    folderPath = folderPath.TrimEnd(new char[] { '/' });
                }

                return folderPath;
            }
        }

        public static string FindTypeFromSignature(string signature)
        {
            if (string.IsNullOrEmpty(signature))
            {
                return string.Empty;
            }
            else
            {
                return signature.Substring(0, signature.IndexOf(":"));
            }
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

    }
}
