using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Navitaire.Reporting.Services.Authentication;
using Navitaire.Reporting.Services.Exceptions;
using Navitaire.Reporting.Services.Catalog;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class ExportMapManager
    {
        private AuthenticatedUserManager _owner;

        private BOExportMapCollection _exportMapCollection;

        #region Constructors

        private ExportMapManager()
        {
            // Hide the default constructor
        }

        public ExportMapManager(AuthenticatedUserManager owner)
        {
            _owner = owner;
        }

        #endregion Constructors

        public BOExportMapCollection ExportMapCollection
        {
            get
            {
                if (_exportMapCollection == null)
                {
                    _exportMapCollection = new BOExportMapCollection();
                    _exportMapCollection.Fill();
                }

                return _exportMapCollection;
            }
        }

        #region Private Helper Methods

        private string buildCommandArguments(Hashtable newProcArgs)
        {
            // Substitute "NULL" for any empty/null value in the parameter table.  This is so we 
            // can call the stored proc with the correct arguments.
            StringBuilder argListBuilder = new StringBuilder();
            foreach (string paramNameKey in newProcArgs.Keys)
            {
                string paramValue = (string)newProcArgs[paramNameKey];
                if (string.IsNullOrEmpty(paramValue))
                {
                    argListBuilder.AppendFormat("{0} = NULL, ", paramNameKey);
                }
                else
                {
                    argListBuilder.AppendFormat("{0} = '{1}', ", paramNameKey, paramValue);
                }
            }

            // Remove the last comma
            char[] commaTrim = { ',', ' ' };
            return argListBuilder.ToString().TrimEnd(commaTrim);
        }

        private string lookupConfiguredConnectionString(BOExportMap exportMap)
        {
            // Find the datasource to make the connection to the DB
            BOConfiguredDataSource configuredDataSource = _owner.CatalogInteractionManager.ConfiguredDataSourceCollection[exportMap.DataSourcePath];
            if (configuredDataSource == null)
            {
                throw new InvalidConfigurationException("The datasource for this report has not been configured for the application.");
            }

            // Remove any trailing semi-colons so we can append the credentials to the connection string
            string trimmedConnectionString = configuredDataSource.ConnectionStringValue.TrimEnd(new char[] { ';' });

            // Decrypt the credentials stored in the DB
            string plainCrednetials = Utility.DecryptValue(configuredDataSource.Credentials);

            // Put the pieces together
            return string.Format(CultureInfo.InvariantCulture, "{0};{1}", trimmedConnectionString, plainCrednetials);
        }

        private Hashtable lookupStoredProcParametersFromRdl(BOExportMap exportMap)
        {
            Hashtable storedProcParameters = new Hashtable();

            // Get Report RDL
            byte[] rdl = _owner.ReportingServiceNonHttpClient.GetReportDefinition(exportMap.ReportPath);

            // Parse RDL to find proc definition -- need to find out which DS is the primary one.
            XmlDocument rdlDocument = new XmlDocument();
            rdlDocument.Load(new MemoryStream(rdl));

            // RDL is a well-formed XML doc, add a namespace manager so we can use XPath to search it
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(rdlDocument.NameTable);
            nsmgr.AddNamespace("default", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");

            XmlNodeList dataSetNodeList = rdlDocument.DocumentElement.SelectNodes("//default:Report/default:DataSets/default:DataSet", nsmgr);

            foreach (XmlNode dataSetNode in dataSetNodeList)
            {
                bool foundMainDataSet = false;

                bool isStoredProc = false;
                bool isDWDataSource = false;
                bool isCorrectStoredProc = false;

                XmlNode queryCommandTypeNode = dataSetNode.SelectSingleNode("./default:Query/default:CommandType", nsmgr);
                if (queryCommandTypeNode != null)
                {
                    if (queryCommandTypeNode.InnerText.Equals("StoredProcedure", StringComparison.OrdinalIgnoreCase))
                    {
                        isStoredProc = true;
                    }
                }
                XmlNode queryDataSourceNameNode = dataSetNode.SelectSingleNode("./default:Query/default:DataSourceName", nsmgr);
                if (queryDataSourceNameNode != null)
                {
                    if (queryDataSourceNameNode.InnerText.Equals("DataWarehouse", StringComparison.OrdinalIgnoreCase))
                    {
                        isDWDataSource = true;
                    }
                }

                if (isStoredProc)
                {
                    XmlNode queryCommandTextNode = dataSetNode.SelectSingleNode("./default:Query/default:CommandText", nsmgr);
                    if (queryCommandTextNode != null)
                    {
                        string storedProcName = queryCommandTextNode.InnerText.Trim().Replace("[", null).Replace("]", null);
                        if (exportMap.ProcName.Equals(storedProcName, StringComparison.OrdinalIgnoreCase))
                        {
                            isCorrectStoredProc = true;
                        }
                    }
                }

                if ((isStoredProc) && (isDWDataSource) && (isCorrectStoredProc))
                {
                    foundMainDataSet = true;
                }

                if (foundMainDataSet)
                {
                    XmlNodeList queryCommandParameterNodeList = dataSetNode.SelectNodes("./default:Query/default:QueryParameters/default:QueryParameter", nsmgr);
                    if (queryCommandParameterNodeList != null)
                    {
                        foreach (XmlNode queryCommandParameterNode in queryCommandParameterNodeList)
                        {
                            string paramName = ((XmlElement)queryCommandParameterNode).Attributes["Name"].Value;

                            string paramValue = string.Empty;
                            foreach (XmlNode parameterChildNode in queryCommandParameterNode.ChildNodes)
                            {
                                if (parameterChildNode.Name == "Value")
                                {
                                    paramValue = parameterChildNode.InnerText;
                                }
                            }
                            storedProcParameters.Add(paramName, paramValue);
                        }
                    }

                    break;
                }
            }

            return storedProcParameters;
        }

        private static Hashtable mergeParameters(Hashtable storedProcParameters, NParameterValueCollection parameterValueCollection, CultureInfo culInfo, string strUser)
        {
            Hashtable mergedProcArgs = new Hashtable();

            foreach (string procArgKey in storedProcParameters.Keys)
            {
                string adjValue = null;

                char[] argTrimChar = { '@' };
                string keyTest = procArgKey.TrimStart(argTrimChar);
                string origValue = (string)storedProcParameters[procArgKey];
                if (parameterValueCollection[keyTest] != null)
                {
                    // C# is case sensitive, RDL isn't. Convert all to upper for comparisons.
                    string upperOrig = origValue.ToUpperInvariant();

                    // The RDL can contain spaces between = and the start of the line.  So remove 
                    // spaces so we can check the start syntax of the param definition.
                    string upperOrigNoSpace = upperOrig.Replace(" ", null).Replace("\t", null);

                    // Update the Proc Arguments - Search for patterns to try to reproduce the behavior we get from SRS
                    // Note: This is a hack.  We should implement the report parameters ourselves.
                    string testValue = parameterValueCollection[keyTest].Value;

                    if (upperOrigNoSpace.StartsWith("=JOIN", StringComparison.OrdinalIgnoreCase))
                    {
                        // Task159569 - Get the value as it is
                        adjValue = testValue;
                    }

                    else if (upperOrigNoSpace.StartsWith("=PARAMETERS!", StringComparison.OrdinalIgnoreCase))
                    {
                        // If the adjusted value is a Date, the string usually contains an AM/PM time stamp, remove that to allow for inter-day.
                        DateTime testDate;

                        // Task 176511 - Added condition to exclude values with comma. These are multi-select values.
                        if (!string.IsNullOrEmpty(testValue))
                        {
                            if (!testValue.Contains(","))
                            {
                                if (DateTime.TryParse(testValue, culInfo, DateTimeStyles.None, out testDate))
                                {
                                    testValue = testDate.ToShortDateString();
                                }
                            }
                        }

                        // straight substitution
                        adjValue = testValue;
                    }
                    else if ((upperOrigNoSpace.StartsWith("=CODE.GLOBALIZATION.GETENDDATE", StringComparison.OrdinalIgnoreCase)) ||
                              (upperOrigNoSpace.StartsWith("=CODE.UTILITY.GETENDDATE", StringComparison.OrdinalIgnoreCase)))
                    {
                        //Task 170701 - Convert DateTime based on its culture
                        DateTime conv;
                        string strDate = testValue.ToString();
                        conv = GetEndDate(Convert.ToDateTime(strDate, culInfo));

                        // Set the DateTime back to string using EN-US formating
                        string strTime = conv.TimeOfDay.Hours.ToString() + ":" + conv.TimeOfDay.Minutes.ToString() + ":" + conv.TimeOfDay.Seconds.ToString() + "." + conv.TimeOfDay.Milliseconds.ToString();
                        adjValue = string.Format("{0} {1}", conv.ToShortDateString(), strTime);

                    }
                    // Task172755 - Add the User!UserID functionality
                    else if (upperOrigNoSpace.StartsWith("=User", StringComparison.OrdinalIgnoreCase))
                    {
                        adjValue = strUser;
                    }
                    else if (upperOrigNoSpace.StartsWith("=IIF", StringComparison.OrdinalIgnoreCase))
                    {
                        if (((upperOrig.Contains("   1")) || (upperOrig.Contains("9999") || (upperOrig.Contains("NOTHING")))) &&
                            (upperOrig.Contains("SPACE")) &&
                            (upperOrig.Contains("4")) &&
                            (upperOrig.Contains("LEN")))
                        {
                            #region As It Appears in RDL
                            //=IIF(Parameters!startFlightNumber.Value = "", "   1", SPACE(4-LEN(Parameters!startFlightNumber.Value)) + Parameters!startFlightNumber.Value)
                            //=IIF(Parameters!endFlightNumber.Value = "", "9999", SPACE(4-LEN(Parameters!endFlightNumber.Value)) + Parameters!endFlightNumber.Value)
                            //=IIF(Parameters!flightNumber.Value = "", NOTHING , SPACE(4-LEN(Parameters!flightNumber.Value)) + Parameters!flightNumber.Value)
                            #endregion As It Appears in RDL

                            if (string.IsNullOrEmpty(testValue))
                            {
                                if (upperOrig.Contains("   1"))
                                {
                                    // Start Flight Number
                                    adjValue = "   1";
                                }
                                else if (upperOrig.Contains("9999"))
                                {
                                    // End Flight Number
                                    adjValue = "9999";
                                }
                                else if (upperOrig.Contains("NOTHING"))
                                {
                                    // Flight Number
                                    adjValue = null;
                                }
                            }
                            else
                            {
                                // Padd the specified flight number with up to 3 spaces
                                string padding = string.Empty;
                                int paddLength = 4 - testValue.Length;
                                if (paddLength > 0)
                                {
                                    padding = new string(' ', paddLength);
                                }

                                adjValue = string.Format(CultureInfo.InvariantCulture, "{0}{1}", padding, testValue);
                            }
                        }
                        else
                        {
                            #region As It Appears in RDL
                            //=IIF(Parameters!agentName.Value="",nothing,Parameters!agentName.Value)
                            #endregion As It Appears in RDL

                            // check for empty, if not substitute
                            if (!string.IsNullOrEmpty(testValue))
                            {
                                adjValue = testValue;
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidReportParameterValueException(string.Format(CultureInfo.InvariantCulture, "Unrecognized Parameter Value: '{0}'", upperOrigNoSpace));
                    }
                }
                else
                {
                    // The parameter wasn't found on the URL, check the arguments to the proc to 
                    // see if it was hard coded.
                    string procSpecValue = storedProcParameters[procArgKey].ToString();
                    bool bValid = false;

                    // Task172755 - Add the User!UserID functionality
                    if (procSpecValue.StartsWith("=User", StringComparison.OrdinalIgnoreCase))
                    {
                        adjValue = strUser;
                        bValid = true;
                    }

                    // Task159569 - Add exception for getenddate functionality
                    if (procSpecValue.StartsWith("=code.utility.getenddate", StringComparison.OrdinalIgnoreCase))
                    {
                        // Since there is no reference in the RDL parameter, get the value from startdate
                        string ParamValue = procSpecValue.Substring(36); //get the parameter name
                        ParamValue = ParamValue.Remove(ParamValue.Length - 7); //remove ".value"
                        string testValue = parameterValueCollection[ParamValue].Value;

                        // Task185567 - Added culture in conversion to date
                        DateTime conv = GetEndDate(Convert.ToDateTime(testValue, culInfo));

                        string strTime = conv.TimeOfDay.Hours.ToString() + ":" + conv.TimeOfDay.Minutes.ToString() + ":" + conv.TimeOfDay.Seconds.ToString() + "." + conv.TimeOfDay.Milliseconds.ToString();
                        adjValue = string.Format("{0} {1}", conv.ToShortDateString(), strTime);

                        bValid = true;
                    }

                    // If the parameter isn't a hard-coded value, it should have been passed in 
                    // via the URL
                    if ((procSpecValue.StartsWith("=Parameters.", StringComparison.OrdinalIgnoreCase) ||
                        procSpecValue.StartsWith("=IIF", StringComparison.OrdinalIgnoreCase) ||
                        procSpecValue.StartsWith("=CODE.", StringComparison.OrdinalIgnoreCase)
                        ) && !bValid)
                    {
                        throw new InvalidValueException(string.Format(CultureInfo.InvariantCulture, "The specified Procedure argument '{0}' was not specified on the command line when it should have been.", procSpecValue));
                    }

                    // Remove the leading = char
                    if (procSpecValue.StartsWith("=", StringComparison.OrdinalIgnoreCase) && !bValid)
                    {
                        procSpecValue = procSpecValue.Substring(1);
                    }

                    // Remove escaped Quotes
                    if (!bValid)
                        adjValue = procSpecValue.Replace("\"", string.Empty);
                }

                mergedProcArgs.Add(procArgKey, adjValue);
            }

            return mergedProcArgs;
        }

        #endregion Private Helper Methods

        public BOExportMap FindAssociatedExportMap(string reportPath, bool useHttpClient)
        {
            BOExportMap exportMap = null;

            string itemType = _owner.CatalogInteractionManager.LookupItemType(reportPath, useHttpClient);

            switch (itemType)
            {
                case Constants.CatalogItemTypes.Report:
                    {
                        exportMap = ExportMapCollection[reportPath];
                    }
                    break;
                case Constants.CatalogItemTypes.LinkedReport:
                    {
                        string parentReportPath = _owner.CatalogInteractionManager.LookupReportLink(reportPath, useHttpClient);
                        exportMap = ExportMapCollection[parentReportPath];
                    }
                    break;
            }

            return exportMap;
        }

        public BOExportMap FindAssociatedExportMapItem(string reportPath, bool useHttpClient, string storedProc)
        {
            BOExportMap exportMap = null;
            string itemType = _owner.CatalogInteractionManager.LookupItemType(reportPath, useHttpClient);

            switch (itemType)
            {
                case Constants.CatalogItemTypes.Report:
                    {
                        exportMap = ExportMapCollection[reportPath, storedProc];
                    }
                    break;
                case Constants.CatalogItemTypes.LinkedReport:
                    {
                        string parentReportPath = _owner.CatalogInteractionManager.LookupReportLink(reportPath, useHttpClient);
                        exportMap = ExportMapCollection[parentReportPath, storedProc];
                    }
                    break;
            }

            return exportMap;
        }

        public BOExportMapCollection FindAssociatedExportMapCollection(string reportPath)
        {
            BOExportMapCollection exportMapCollection = null;
            exportMapCollection = ExportMapCollection[reportPath, true];
            return exportMapCollection;
        }

        /// <summary>
        /// This method executes the stored procedure defined in this class via a data reader.  
        /// The parameters from this stored procedure are a combination of the values selected by 
        /// the user through the shell, and the information found in the RDL defined by _reportPath 
        /// </summary>
        /// <param name="user">The currently authenticated NewSkies user</param>
        /// <param name="paramArgTable">Table of name value pairs of parameters to be used for the current execution.  User input from the shell, rolled-dates + user input for subscriptions</param>
        /// <param name="useExternalHandler">Flag that indicates whether the export will be handled by an event outside of this method, or if the method needs to generate the results</param>
        /// <returns></returns>
        public byte[] ExportToCSV(BOExportMap exportMap,
            NParameterValueCollection parameterValueCollection,
            IFormatProvider cultureInfo,
            bool forceError,
            string strUser,
            string strCSVDelimiter)
        {
            byte[] results = null;
            CultureInfo subscriptionCultureInfo = (CultureInfo)cultureInfo;

            // Find the connection string for this exportMap
            string connString = lookupConfiguredConnectionString(exportMap);

            // Get the Stored Proc Parameters from the RDL
            Hashtable storedProcParameters = lookupStoredProcParametersFromRdl(exportMap);

            // Task 176511 - Applied the combinedMethod for multi-select values
            NParameterValueCollection combinedParameterValue;
            combinedParameterValue = combinedMethod(parameterValueCollection);

            Hashtable newProcArgs;
            try
            {
                // Merge the args from the parameters control with the ones from the stored proc in 
                // the RDL
                newProcArgs = mergeParameters(storedProcParameters, combinedParameterValue, subscriptionCultureInfo, strUser);
            }
            catch
            {
                // Merge the args from the parameters control with the ones from the stored proc in 
                // the RDL
                newProcArgs = mergeParameters(storedProcParameters, parameterValueCollection, subscriptionCultureInfo, strUser);
            }

            // Build the argument list used when calling the proc
            string argList = buildCommandArguments(newProcArgs);

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandTimeout = 18000; // Wait for up to 5 hours

                        sqlCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "EXEC {0} {1}", exportMap.ProcName, argList);
                        if (forceError)
                        {
                            // Force error is used for debugging. It returns the text of the query 
                            // to the caller as the message of the exception.
                            throw new DiagnosisException(sqlCommand.CommandText);
                        }

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            try
                            {
                                StringBuilder csvBuilder = new StringBuilder();

                                // Header
                                DataTable schemaTable = sqlDataReader.GetSchemaTable();
                                for (int rowNdx = 0; rowNdx < schemaTable.Rows.Count; rowNdx++)
                                {
                                    DataRow schemaRow = schemaTable.Rows[rowNdx];

                                    csvBuilder.Append(schemaRow["ColumnName"]);

                                    // Don't insert a comma at the end of the line
                                    if (rowNdx < schemaTable.Rows.Count - 1)
                                    {
                                        //Task200712 - Applied the CSV delimiter based on the app setting
                                        csvBuilder.Append(strCSVDelimiter);
                                    }
                                    else
                                    {
                                        csvBuilder.Append(Environment.NewLine);
                                    }
                                }

                                // Data
                                while (sqlDataReader.Read())
                                {
                                    for (int colNdx = 0; colNdx < sqlDataReader.FieldCount; colNdx++)
                                    {
                                        string colVal = string.Empty;

                                        Type colType = sqlDataReader[colNdx].GetType();
                                        if (colType == typeof(DateTime))
                                        {
                                            colVal = ((DateTime)sqlDataReader[colNdx]).ToString(cultureInfo);
                                        }
                                        else
                                        {
                                            colVal = sqlDataReader[colNdx].ToString();
                                        }

                                        // Escape any fields that contains commas
                                        if (colVal.Contains(strCSVDelimiter))
                                        {
                                            colVal = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", colVal);
                                        }
                                        csvBuilder.Append(colVal);

                                        // Don't insert a comma at the end of the line
                                        if (colNdx < sqlDataReader.FieldCount - 1)
                                        {
                                            //Task200712 - Applied the CSV delimiter based on the app setting
                                            csvBuilder.Append(strCSVDelimiter);
                                        }
                                        else
                                        {
                                            csvBuilder.Append(Environment.NewLine);
                                        }
                                    }
                                }

                                results = Encoding.Default.GetBytes(csvBuilder.ToString());
                            }
                            finally
                            {
                                sqlDataReader.Close();
                            }
                        }
                    }
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return results;
        }

        private NParameterValueCollection combinedMethod(NParameterValueCollection nvrc)
        {
            //Task 145663 - combined all values of the same parameter (multi select)
            NParameterValueCollection nvrc2 = new NParameterValueCollection();
            string strCombineValue = "";
            bool bKey = false;
            int nCount = nvrc.Count;

            for (int a = 0; a < nCount; a++)
            {
                string strName = string.Empty;
                NParameterValue updateParameter = null;

                strName = nvrc[a].Name;
                //Check if this is the last of the collection
                if ((nCount - a) != 1)
                {
                    //Check next label if same with current
                    if ((strName == nvrc[a + 1].Name))
                    {
                        if (!bKey)
                        {
                            strCombineValue = nvrc[a].Value + "," + nvrc[a + 1].Value;
                            bKey = true;
                        }
                        else
                        {
                            //Append the value of the next parameter to the current parameter
                            strCombineValue = strCombineValue + "," + nvrc[a + 1].Value;
                        }
                    }
                    else
                    {
                        //Create a new parameter
                        if (strCombineValue == "")
                        {
                            updateParameter = nvrc[a];
                        }
                        else
                        {
                            updateParameter = new NParameterValue(nvrc[a].Label,
                                nvrc[a].Name,
                                strCombineValue,
                                nvrc[a].ParameterType,
                                nvrc[a].UseDefault);
                        }
                        nvrc2.Add(updateParameter);
                        //Refresh reference
                        bKey = false;
                        strCombineValue = "";
                    }
                }
                else
                {
                    //Create a new parameter
                    if (strCombineValue == "")
                    {
                        updateParameter = nvrc[a];
                    }
                    else
                    {
                        updateParameter = new NParameterValue(nvrc[a].Label,
                            nvrc[a].Name,
                            strCombineValue,
                            nvrc[a].ParameterType,
                            nvrc[a].UseDefault);
                    }
                    nvrc2.Add(updateParameter);
                }
            }
            return nvrc2;
        }

        /// <summary>
        /// Copy of the GetEndDate method from extensions.
        /// </summary>
        /// <remarks>
        /// This is here to prevent a circular reference between Common and Extensions assemblies
        /// </remarks>
        /// <param name="dateEntered"></param>
        /// <returns></returns>
        public static DateTime GetEndDate(DateTime dateEntered)
        {
            if (dateEntered.TimeOfDay == TimeSpan.Zero)
            {
                dateEntered = dateEntered.AddDays(1).AddMilliseconds(-3);
            }

            return dateEntered;
        }

        /// <summary>
        /// Examine the specified report and return the first datasource encountered
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        public string LookupDataSourceFromRdl(string reportPath)
        {
            #region Validate Parameters
            if (string.IsNullOrEmpty(reportPath))
            {
                throw new ArgumentNullException("reportPath");
            }
            #endregion Validate Parameters

            string itemType = _owner.CatalogInteractionManager.LookupItemType(reportPath, false);

            // The only thing that can be Mapped are Reports, if the specified item is a Linked 
            // report or anything else, return an exception.
            if (!itemType.Equals(Constants.CatalogItemTypes.Report, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidCatalogItemTypeException(string.Format(CultureInfo.InvariantCulture, "The specified item '{0}' is of type '{1}', only objects of type Report can be mapped.", reportPath, itemType));
            }

            string folderPath = CatalogInteractionManager.LookupFolderName(reportPath);
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new InvalidValueException(string.Format(CultureInfo.InvariantCulture, "Unable to locate the folder for the specified report '{0}'", reportPath));
            }

            // Get Report RDL
            byte[] rdl = _owner.ReportingServiceNonHttpClient.GetReportDefinition(reportPath);

            // Parse RDL to find proc definition -- need to find out which DS is the primary one.
            XmlDocument rdlDocument = new XmlDocument();
            rdlDocument.Load(new MemoryStream(rdl));

            // RDL is a well-formed XML doc, add a namespace manager so we can use XPath to search it
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(rdlDocument.NameTable);
            nsmgr.AddNamespace("default", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");

            XmlNode dataSourceReferenceNode = rdlDocument.DocumentElement.SelectSingleNode("//default:Report/default:DataSources/default:DataSource/default:DataSourceReference", nsmgr);

            if (dataSourceReferenceNode == null)
            {
                throw new InvalidValueException(string.Format(CultureInfo.InvariantCulture, "Unable to find datasources in the report '{0}'", reportPath));
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", folderPath, dataSourceReferenceNode.InnerText);
        }

        /// <summary>
        /// Examine the specified report and return a list of all stored procedures contained in it
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        public StringCollection LookupStoredProcFromRdl(string reportPath)
        {
            StringCollection procList = new StringCollection();
            try
            {
                // Get Report RDL
                byte[] rdl = _owner.ReportingServiceNonHttpClient.GetReportDefinition(reportPath);

                // Parse RDL to find proc definition -- need to find out which DS is the primary one.
                XmlDocument rdlDocument = new XmlDocument();
                rdlDocument.Load(new MemoryStream(rdl));

                // RDL is a well-formed XML doc, add a namespace manager so we can use XPath to search it
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(rdlDocument.NameTable);
                nsmgr.AddNamespace("default", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");

                XmlNodeList dataSetNodeList = rdlDocument.DocumentElement.SelectNodes("//default:Report/default:DataSets/default:DataSet", nsmgr);

                foreach (XmlNode dataSetNode in dataSetNodeList)
                {
                    XmlNode queryCommandTypeNode = dataSetNode.SelectSingleNode("./default:Query/default:CommandType", nsmgr);
                    if (queryCommandTypeNode != null)
                    {
                        if (queryCommandTypeNode.InnerText == "StoredProcedure")
                        {
                            XmlNode queryCommandTextNode = dataSetNode.SelectSingleNode("./default:Query/default:CommandText", nsmgr);
                            if (queryCommandTextNode != null)
                            {
                                string storedProcName = queryCommandTextNode.InnerText.Trim().Replace("[", null).Replace("]", null);
                                procList.Add(storedProcName);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If it blows up, don't return a list
            }

            return procList;
        }
    }
}
