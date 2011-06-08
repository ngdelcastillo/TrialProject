using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.ReportingServices.ReportService2005;

namespace Navitaire.Reporting.Services.Catalog
{
    public class NValidValue
    {
        #region Private Variables
        public string Label { get; set; }
        public string Value { get; set; }
        #endregion

        #region Constructors
        public NValidValue(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public NValidValue(ValidValue validValue)
        {
            Label = validValue.Label;
            Value = validValue.Value;
        }
        #endregion
    }
}
