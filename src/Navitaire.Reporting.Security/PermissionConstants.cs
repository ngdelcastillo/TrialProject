using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navitaire.Reporting.Security
{
    public class PermissionConstants
    {
        public static class PermissionName
        {
            public const string CreateLinkedReports = "Reporting.Reports.CreateLinkedReports";
            public const string CreateSubscriptions = "Reporting.Reports.ManageIndividualSubscriptions";
            public const string ManageDataSources = "Reporting.Reports.ManageDataSources";
            public const string ManageFolders = "Reporting.Reports.ManageFolders";
            public const string ManageAllSubscriptions = "Reporting.Reports.ManageAllSubscriptions";
            public const string ManageReports = "Reporting.Reports.ManageReports";
            public const string ManageReportServerProperties = "Reporting.Reports.ManageReportServerProperties";
        }
    }
}
