using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    /// <summary>
    /// BOExportMapCollection contains custom business logic associated with BOExportMapCollectionGen
    /// </summary>
    public class BOExportMapCollection : BOExportMapCollectionGen
    {
        public BOExportMap this[Guid exportMapId]
        {
            get
            {
                foreach (BOExportMap exportMap in this)
                {
                    if (exportMap.ExportMapId.Equals(exportMapId))
                    {
                        return exportMap;
                    }
                }

                return null;
            }
        }

        public BOExportMap this[string reportPath]
        {
            get
            {
                foreach (BOExportMap exportMap in this)
                {
                    if (exportMap.ReportPath.Equals(reportPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return exportMap;
                    }
                }

                return null;
            }
        }

        public BOExportMap this[string reportPath, string storedProc]
        {
            get
            {
                foreach (BOExportMap exportMap in this)
                {
                    if ((exportMap.ReportPath.Equals(reportPath, StringComparison.OrdinalIgnoreCase)) && (exportMap.ProcName.Equals(storedProc, StringComparison.OrdinalIgnoreCase)))
                    {
                        return exportMap;
                    }
                }

                return null;
            }
        }

        public BOExportMapCollection this[string reportPath, bool collection]
        {
            get
            {
                BOExportMapCollection exportMapCollection = new BOExportMapCollection();
                bool bKey = false;
                foreach (BOExportMap exportMap in this)
                {
                    if (exportMap.ReportPath.Equals(reportPath, StringComparison.OrdinalIgnoreCase))
                    {
                        exportMapCollection.Add(exportMap);
                        //Only make true if the collection is more than 1
                        if (exportMapCollection.Count > 1)
                        {
                            bKey = true;
                        }
                    }
                }

                if (bKey)
                {
                    return exportMapCollection;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Fill()
        {
            this.Clear();

            this.SelectExportMapList();
        }

        public void FillTarget()
        {
            this.Clear();
            this.SelectTargetExportMapList();
        }

        public bool Remove(Guid exportMapId)
        {
            BOExportMap theExportMap = this[exportMapId];
            if (theExportMap != null)
            {
                return this.Remove(theExportMap);
            }

            return true;
        }
    }
}
