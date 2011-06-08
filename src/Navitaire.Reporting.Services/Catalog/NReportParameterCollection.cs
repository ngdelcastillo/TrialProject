using System;
using System.Collections.ObjectModel;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NReportParameterCollection : Collection<NReportParameter>
    {
        /// <summary>
        /// Returns a value indicating if the paremeterName supplied has other paremeters in the 
        /// report that are dependent on it
        /// </summary>
        public bool HasDependencies(string parameterName)
        {
            foreach (NReportParameter parameter in this)
            {
                foreach (string dependency in parameter.Dependencies)
                {
                    if (dependency.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public NReportParameter this[string parameterName]
        {
            get
            {
                foreach (NReportParameter parameter in this)
                {
                    if (parameter.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        return parameter;
                    }
                }
                return null;
            }
        }
    }
}
