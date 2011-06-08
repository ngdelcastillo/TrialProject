using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

using Navitaire.Reporting.Security;
using Navitaire.Reporting.Security.Exceptions;
using Navitaire.Reporting.Services.SRS;
using Navitaire.Reporting.Services.Authentication;
using Navitaire.Reporting.Services.BusinessObjects;
using Navitaire.Reporting.Services.Exceptions;

using Microsoft.SqlServer.ReportingServices.ReportService2005;

namespace Navitaire.Reporting.Services.Catalog
{
    public class CatalogInteractionManager
    {
        #region Private Variables
        private AuthenticatedUserManager _authenticatedUserManager;        
        private BOConfiguredDataSourceCollection _configuredDatasources;
        #endregion

        #region Properties
        public BOConfiguredDataSourceCollection ConfiguredDatasources
        {
            get
            {
                if (_configuredDatasources == null)
                {
                    _configuredDatasources = new BOConfiguredDataSourceCollection();
                    _configuredDatasources.Fill();
                }
                return _configuredDatasources;
            }
        }

        #region Internal
        public ReportingServiceClient ReportingServiceClient
        {
            get
            {
                return _authenticatedUserManager.ReportingServiceClient;
            }
        }

        public ReportingServiceNonHttpClient ReportingServiceNonHttpClient
        {
            get
            {
                return _authenticatedUserManager.ReportingServiceNonHttpClient;
            }
        }

        public ReportingServiceClient ReportingTargetServiceClient
        {
            get
            {
                return _authenticatedUserManager.ReportingTargetServiceClient;
            }
        }

        public ReportingServiceNonHttpClient ReportingTargetServiceNonHttpClient
        {
            get
            {
                return _authenticatedUserManager.ReportingTargetServiceNonHttpClient;
            }
        }

        public ReportingServiceClient ReportingSourceServiceClient
        {
            get
            {
                return _authenticatedUserManager.ReportingSourceServiceClient;
            }
        }

        public ReportingServiceNonHttpClient ReportingSourceServiceNonHttpClient
        {
            get
            {
                return _authenticatedUserManager.ReportingSourceServiceNonHttpClient;
            }
        }
        #endregion

        #endregion

        #region Constructors
        public CatalogInteractionManager(AuthenticatedUserManager user)
        {
            #region Validate Parameters
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            #endregion
            _authenticatedUserManager = user;
        }
        #endregion

        #region Methods
        public void AssignDataSourceReferences(string itemPath, NDatasourceReferenceCollection dataSourceReferences)
        {
            List<DataSource> dataSourceList = new List<DataSource>();

            ///////////////////////////////////////////////////////////////////////////////////////
            // Note: According to documentation, only datasources that are being modified need to 
            //       be included.
            ///////////////////////////////////////////////////////////////////////////////////////

            // Add the links.
            foreach (NDatasourceReference modifiedReference in dataSourceReferences)
            {
                DataSourceReference newDataSourceReference = new DataSourceReference();
                newDataSourceReference.Reference = modifiedReference.DatasourcePath;

                DataSource newDataSource = new DataSource();
                newDataSource.Name = modifiedReference.Name;
                newDataSource.Item = newDataSourceReference;

                dataSourceList.Add(newDataSource);
            }

            DataSource[] dataSources = new DataSource[dataSourceList.Count];
            dataSourceList.CopyTo(dataSources);

            this.ReportingServiceClient.SetItemDataSources(itemPath, dataSources);
        }

        public NCatalogItemCollection BuildDependencyList(string catalogItemPath)
        {
            NCatalogItemCollection dependencyList = new NCatalogItemCollection();
            
            // Get a list of any child dependentancy items
            NCatalogItemCollection childCatalogItemList = null;

            // Get the ItemType so we can behave appropriately
            string itemType = LookupItemType(catalogItemPath, true);
            if (itemType == Constants.CatalogItemTypes.Folder)
            {
                childCatalogItemList = LookupCatalogItems(catalogItemPath, true, true, null);
            }
            else
            {
                childCatalogItemList = LookupCatalogItems(null, true, true, null);
            }
            addDependencies(dependencyList, catalogItemPath, itemType, childCatalogItemList);
            return dependencyList;
        }

        public NDatasource CreateDataSource(string dataSourceName,
            string parentFolderPath,
            string provider,
            string server,
            string database,
            string userName,
            string password,
            string passwordConfirm,
            bool useHttpClient)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(dataSourceName))
            {
                throw new ArgumentNullException("dataSourceName");
            }
            if (string.IsNullOrEmpty(parentFolderPath))
            {
                throw new ArgumentNullException("parentFolderPath");
            }
            if (!("SQL".Equals(provider)))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Provider type {0} is not supported.", provider));
            }
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentNullException("server");
            }
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentNullException("database");
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }
            if (!password.Equals(passwordConfirm))
            {
                throw new ArgumentException("Passwords do not match.");
            }

            #endregion Validate Parameters

            string connectionString = string.Format(CultureInfo.InvariantCulture, @"data source={0};initial catalog={1}", server, database);

            // Create the SRS version of this data source
            Property[] properties = null;
            DataSourceDefinition definition = new DataSourceDefinition();
            definition.ConnectString = connectionString;
            definition.CredentialRetrieval = CredentialRetrievalEnum.Store;
            definition.Enabled = true;
            definition.EnabledSpecified = true;
            definition.Extension = provider;
            definition.ImpersonateUser = false;
            definition.ImpersonateUserSpecified = false;
            definition.UserName = userName;
            definition.Password = password;            

            if (useHttpClient)
            {
                this.ReportingServiceClient.CreateDataSource(dataSourceName, parentFolderPath, true, definition, properties);
            }
            else
            {
                this.ReportingServiceNonHttpClient.CreateDataSource(dataSourceName, parentFolderPath, true, definition, properties);
            }

            // Create the application version of this data source
            bool isNewDataSource = false;
            string catalogPath = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", parentFolderPath, dataSourceName);

            BOConfiguredDataSource configuredDataSource = ConfiguredDataSourceCollection[catalogPath];
            if (configuredDataSource == null)
            {
                isNewDataSource = true;
                configuredDataSource = new BOConfiguredDataSource();
            }
            configuredDataSource.CatalogPath = catalogPath;
            configuredDataSource.Credentials = Utility.EncryptValue(string.Format(CultureInfo.InvariantCulture, "uid={0};pwd={1}", userName, password));
            configuredDataSource.ConnectionStringValue = connectionString;
            configuredDataSource.ConnectionStringType = provider;
            configuredDataSource.Save(_authenticatedUserManager.AuthenticatedUserSignature);

            if (isNewDataSource)
            {
                // Add it to the cache
                this.ConfiguredDataSourceCollection.Add(configuredDataSource);
            }

            string dataSourcePath = string.Format("{0}/{1}", parentFolderPath, dataSourceName);
            return new NDatasource(dataSourceName, dataSourcePath);
        }


        public NFolder CreateFolder(string folderName, string parentFolderPath, bool useHttpClient)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }
            else if (string.IsNullOrEmpty(parentFolderPath))
            {
                throw new ArgumentNullException("parentFolderPath");
            }

            #endregion Validate Parameters
            
            if (!_authenticatedUserManager.CheckUserPermission(PermissionConstants.PermissionName.ManageFolders))
            {
                throw new AccessDeniedException("Unable to create folder.");
            }

            Property[] properties = null;

            if (useHttpClient)
            {
                this.ReportingServiceClient.CreateFolder(folderName, parentFolderPath, properties);
            }
            else
            {
                this.ReportingServiceNonHttpClient.CreateFolder(folderName, parentFolderPath, properties);
            }

            string newFolderPath = string.Empty;
            if (parentFolderPath.Equals("/", StringComparison.OrdinalIgnoreCase))
            {
                // We are at the root path
                newFolderPath = string.Format("/{0}", folderName);
            }
            else
            {
                newFolderPath = string.Format("{0}/{1}", parentFolderPath, folderName);
            }

            return new NFolder(folderName, newFolderPath);
        }

        public NLinkedReport CreateLinkedReport(
            string linkedReportName,
            string folderPath,
            string reportPath,
            bool useHttpClient)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(linkedReportName))
            {
                throw new ArgumentNullException("linkedReportName");
            }
            else if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException("folderPath");
            }
            else if (string.IsNullOrEmpty(reportPath))
            {
                throw new ArgumentNullException("reportPath");
            }

            #endregion ValidateParameters

            if (!_authenticatedUserManager.CheckUserPermission(PermissionConstants.PermissionName.CreateLinkedReports))
            {
                throw new AccessDeniedException("Unable to create linked report.");
            }

            Property[] properties = null;

            if (useHttpClient)
            {
                this.ReportingServiceClient.CreateLinkedReport(
                    linkedReportName,
                    folderPath,
                    reportPath,
                    properties);
            }
            else
            {
                this.ReportingServiceNonHttpClient.CreateLinkedReport(
                    linkedReportName,
                    folderPath,
                    reportPath,
                    properties);
            }

            string linkedReportPath = string.Format("{0}/{1}", folderPath, linkedReportName);
            return new NLinkedReport(linkedReportName, linkedReportPath);
        }

        public void DeleteCatalogItem(string catalogItemPath, bool supressExceptionOnFailure, bool useHttpClient)
        {
            try
            {
                string itemType = LookupItemType(catalogItemPath, useHttpClient);

                if (itemType.Equals(Constants.CatalogItemTypes.Folder, StringComparison.OrdinalIgnoreCase))
                {
                    if (!_authenticatedUserManager.CheckUserPermission(PermissionConstants.PermissionName.ManageFolders))
                    {
                        throw new AccessDeniedException("Unable to delete catalog item");
                    }
                }
                else if (itemType.Equals(Constants.CatalogItemTypes.DataSource, StringComparison.OrdinalIgnoreCase))
                {
                    BOConfiguredDataSource configuredDataSource = ConfiguredDataSourceCollection[catalogItemPath];
                    if (configuredDataSource != null)
                    {
                        configuredDataSource.Delete();
                        ConfiguredDataSourceCollection.Remove(configuredDataSource);
                    }
                }

                if (useHttpClient)
                {
                    this.ReportingServiceClient.DeleteItem(catalogItemPath);
                }
                else
                {
                    this.ReportingServiceNonHttpClient.DeleteItem(catalogItemPath);
                }
            }
            catch
            {
                if (!supressExceptionOnFailure)
                {
                    throw;
                }
            }
        }

        public void DeleteTargetCatalogItem(string catalogItemPath, bool supressExceptionOnFailure, bool useHttpClient)
        {
            try
            {
                if (!useHttpClient)
                {
                    this.ReportingTargetServiceNonHttpClient.DeleteItem(catalogItemPath);
                }
                else
                {
                    this.ReportingTargetServiceClient.DeleteItem(catalogItemPath);
                }
            }
            catch
            {
                if (!supressExceptionOnFailure)
                {
                    throw;
                }
            }
        }

        public byte[] LoadItemBytesFromCatalog(string catalogItemPath, out string mimeType)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(catalogItemPath))
            {
                throw new ArgumentNullException("catalogItemPath");
            }

            #endregion Validate Parameters

            mimeType = string.Empty;
            byte[] contentBytes = null;

            // Inspect the item properties to see what type of object we're dealing with
            ItemTypeEnum catalogItemType = ReportingServiceClient.GetItemType(catalogItemPath);

            switch (catalogItemType)
            {
                case ItemTypeEnum.LinkedReport:
                    {
                        // Find the underlying report
                        string reportPath = ReportingServiceClient.GetReportLink(catalogItemPath);

                        contentBytes = ReportingServiceClient.GetReportDefinition(reportPath);
                        mimeType = Constants.MimeTypes.RDL;
                    }
                    break;
                case ItemTypeEnum.Report:
                    {
                        contentBytes = ReportingServiceClient.GetReportDefinition(catalogItemPath);
                        mimeType = Constants.MimeTypes.RDL;
                    }
                    break;
                case ItemTypeEnum.Resource:
                    {
                        contentBytes = ReportingServiceClient.GetResourceContents(catalogItemPath, out mimeType);
                    }
                    break;
                default:
                    throw new InvalidCatalogItemTypeException(catalogItemType.ToString());
            }

            return contentBytes;
        }

        public NDatasource LookupDataSource(string itemPath, ReportingServiceClient srsProxy)
        {
            NDatasource nDataSource = null;
            DataSourceDefinition dataSourceDefinition = srsProxy.GetDataSourceContents(itemPath);
            if (dataSourceDefinition != null)
            {
                string dataSourceName = itemPath.Substring(itemPath.LastIndexOf(@"/", StringComparison.OrdinalIgnoreCase) + 1);

                // Consume the data source in SQL client to validate that it is the right format
                // and to get the Server and database... instead of writing custom parse code.
                SqlConnection sqlConnection = new SqlConnection(dataSourceDefinition.ConnectString);                
                string server = sqlConnection.DataSource;
                string database = sqlConnection.Database;
                string providerExtension = dataSourceDefinition.Extension;
                string userName = string.Empty;
                string password = string.Empty;
                bool getCredentialsFromSRSDef = true;

                try
                {
                    BOConfiguredDataSource configuredDataSourceDefinition = this.ConfiguredDataSourceCollection[itemPath];
                    if (configuredDataSourceDefinition != null)
                    {
                        string credentials = Utility.DecryptValue(configuredDataSourceDefinition.Credentials);
                        if ((credentials.IndexOf("uid=", StringComparison.OrdinalIgnoreCase) == 0) &&
                            (credentials.IndexOf(";pwd=", StringComparison.OrdinalIgnoreCase) > 0))
                        {
                            string[] credentialPieces = credentials.Split(new char[] { ';' });
                            userName = credentialPieces[0].Split(new char[] { '=' })[1];
                            password = credentialPieces[1].Split(new char[] { '=' })[1];

                            getCredentialsFromSRSDef = false;
                        }
                    }
                }
                catch
                {
                    // Getting credentails from our application's configured credentials failed, use SRS's information
                }

                if (getCredentialsFromSRSDef)
                {
                    if (dataSourceDefinition.ImpersonateUserSpecified)
                    {
                        userName = dataSourceDefinition.UserName;
                        password = dataSourceDefinition.Password;
                    }
                }
                nDataSource = new NDatasource(dataSourceName,
                    itemPath,
                    providerExtension,
                    server,
                    database,
                    userName,
                    password);
            }
            return nDataSource;
        }

        public NDatasourceReferenceCollection LookupDataSourceReferences(string itemPath)
        {
            NDatasourceReferenceCollection dataSourcePaths = new NDatasourceReferenceCollection();

            DataSource[] dataSources = this.ReportingServiceClient.GetItemDataSources(itemPath);
            foreach (DataSource ds in dataSources)
            {
                DataSourceReference refDS = ds.Item as DataSourceReference;
                if (refDS != null)
                {
                    NDatasourceReference nRef = new NDatasourceReference(ds.Name, refDS.Reference);
                    dataSourcePaths.Add(nRef);
                }
            }

            return dataSourcePaths;
        }

        public NameValueCollection LookupItemProperties(string itemPath)
        {
            NameValueCollection itemProperties = new NameValueCollection();
            Property[] propertyArray = this.ReportingServiceClient.GetProperties(itemPath, null);

            foreach (Property property in propertyArray)
            {
                itemProperties.Add(property.Name, property.Value);
            }
            return itemProperties;
        }

        public NLinkedReport UpdatedLinkedReport(string oldLinkedReportPath, string linkedReportName, string folderPath, string reportPath)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(linkedReportName))
            {
                throw new ArgumentNullException("linkedReportName");
            }
            else if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException("folderPath");
            }
            else if (string.IsNullOrEmpty(reportPath))
            {
                throw new ArgumentNullException("reportPath");
            }

            #endregion ValidateParameters

            if (!_authenticatedUserManager.CheckUserPermission(PermissionConstants.PermissionName.CreateLinkedReports))
            {
                throw new AccessDeniedException("Unable to create linked report.");
            }

            // Create a BatchId to allow this multi-step process to succeed/fail as one step.
            BatchHeader updateBatchHeader = new BatchHeader();
            updateBatchHeader.BatchID = ReportingServiceClient.CreateBatch();
            ReportingServiceClient.BatchHeaderValue = updateBatchHeader;

            // Delete the old item
            ReportingServiceClient.DeleteItem(oldLinkedReportPath);

            // Create a new one
            Property[] properties = null;
            ReportingServiceClient.CreateLinkedReport(linkedReportName, folderPath, reportPath, properties);

            // Execute it as batch
            ReportingServiceClient.ExecuteBatch();
            ReportingServiceClient.BatchHeaderValue = null;

            string linkedReportPath = string.Format("{0}/{1}", folderPath, linkedReportName);
            return new NLinkedReport(linkedReportName, linkedReportPath);
        }

        public NCatalogItem UploadFileToCatalog(string folderPath,
            string filePath,
            string catalogItemName,
            bool overwriteExisting,
            byte[] contentDef,
            bool useHttpClient)
        {
            NCatalogItem newCatalogItem = null;
            string extension = Path.GetExtension(filePath);

            if (overwriteExisting)
            {
                // If we selected overwrite, try to delete the item first.  This allows us to update the properties such as description, just specifying true to the overwrite flag won't update the objects properties.
                DeleteCatalogItem(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", folderPath, catalogItemName), true, useHttpClient);
            }

            string itemPath = string.Format("{0}/{1}", folderPath, catalogItemName);
            if (extension.Equals(Constants.CatalogItemExtensions.RDL, StringComparison.OrdinalIgnoreCase))
            {
                Property[] customPropertyArray = null;
                List<Property> customPropertyList = new List<Property>();

                // Inspect the RDL for custom properties
                XmlDocument rdlDocument = new XmlDocument();
                rdlDocument.Load(new MemoryStream(contentDef));

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(rdlDocument.NameTable);
                nsmgr.AddNamespace("default", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");

                XmlNodeList customPropNodeList = rdlDocument.DocumentElement.SelectNodes("//default:Report/default:CustomProperties/default:CustomProperty", nsmgr);
                if (customPropNodeList != null)
                {
                    foreach (XmlNode customPropNode in customPropNodeList)
                    {
                        XmlNode propNameNode = customPropNode.SelectSingleNode("//default:Name", nsmgr);
                        XmlNode propValueNode = customPropNode.SelectSingleNode("//default:Value", nsmgr);

                        if ((propNameNode != null) &&
                             (propValueNode != null))
                        {
                            Property customProp = new Property();
                            customProp.Name = propNameNode.InnerText;
                            customProp.Value = propValueNode.InnerText;

                            customPropertyList.Add(customProp);
                        }
                    }
                }

                if (customPropertyList.Count > 0)
                {
                    customPropertyArray = new Property[customPropertyList.Count];
                    customPropertyList.CopyTo(customPropertyArray);
                }

                if (useHttpClient)
                {
                    this.ReportingServiceClient.CreateReport(catalogItemName,
                        folderPath,
                        overwriteExisting,
                        contentDef,
                        customPropertyArray);
                }
                else
                {
                    this.ReportingServiceNonHttpClient.CreateReport(catalogItemName,
                        folderPath,
                        overwriteExisting,
                        contentDef,
                        customPropertyArray);
                }
                newCatalogItem = new NReport(catalogItemName, itemPath);
            }
            else
            {
                throw new InvalidCatalogItemTypeException(string.Format(CultureInfo.InvariantCulture, "Unrecognized file type '{0}'", extension));
            }
            return newCatalogItem;
        }

        #region Static Methods
        public static string LookupFolderName(string itemPath)
        {
            string itemName = string.Empty;

            itemPath = itemPath.TrimEnd(new char[] { '/' });
            int lastNdx = itemPath.LastIndexOf('/');
            if (lastNdx > 0)
            {
                itemName = itemPath.Substring(0, lastNdx);
            }
            else
            {
                itemName = itemPath;
            }

            return itemName;
        }

        public static string LookupItemName(string itemPath)
        {
            string itemName = string.Empty;

            itemPath = itemPath.TrimEnd(new char[] { '/' });
            int lastNdx = itemPath.LastIndexOf('/');
            if (lastNdx > 0)
            {
                itemName = itemPath.Substring(lastNdx + 1);
            }
            else
            {
                itemName = itemPath;
            }

            return itemName;
        }

        public void MoveItem(string sourcePath, string destPath)
        {
            this.ReportingServiceClient.MoveItem(sourcePath, destPath);
        }


        #endregion

        #region Copy Deploy Functionality
        public DataSourceDefinition GetSRSDataSourcefromSource(NCatalogItem item)
        {
            //CreateDataSource(item.Name, parent, false, 
            DataSourceDefinition definition = this.ReportingSourceServiceNonHttpClient.GetDataSourceContents(item.Path);

            //Get the Password from the app.ConfiguredDatasource
            BOConfiguredDataSourceCollection datasources = new BOConfiguredDataSourceCollection();
            datasources.Fill();
            string[] credentialPieces;
            string userName;
            string password;

            foreach (BOConfiguredDataSource datasource in datasources)
            {
                if (datasource.CatalogPath.Equals(item.Path, StringComparison.OrdinalIgnoreCase))
                {
                    string credential = Utility.DecryptValue(datasource.Credentials);
                    credentialPieces = credential.Split(new char[] { ';' });
                    userName = credentialPieces[0].Split(new char[] { '=' })[1];
                    password = credentialPieces[1].Split(new char[] { '=' })[1];
                    definition.Password = password;
                    break;
                }
            }
            return definition;
        }

        public void CopySRSDataSourceToTarget(NCatalogItem item, string parent, DataSourceDefinition definition)
        {
            this.ReportingTargetServiceNonHttpClient.CreateDataSource(item.Name, parent, false, definition, null);
        }

        public void CopySRSFolderToTarget(NCatalogItem item, string parent)
        {
            this.ReportingTargetServiceNonHttpClient.CreateFolder(item.Name, parent, null);
        }

        public byte[] GetSRSReportFromSource(NCatalogItem item, string parent)
        {
            byte[] rdl = this.ReportingSourceServiceNonHttpClient.GetReportDefinition(item.Path);
            return rdl;
        }

        public void CopySRSReportToTarget(NCatalogItem item, string parent, byte[] rdl)
        {
            this.ReportingTargetServiceNonHttpClient.CreateReport(item.Name, parent, false, rdl, null);
        }

        public string GetSRSLinkReportsFromSource(NCatalogItem item, string parent)
        {
            string link = this.ReportingSourceServiceNonHttpClient.GetReportLink(item.Path);
            return link;
        }

        public void CopySRSLinkReportsToTarget(NCatalogItem item, string parent, string link)
        {
            this.ReportingTargetServiceNonHttpClient.CreateLinkedReport(item.Name, parent, link, null);
        }

        public NCatalogItemCollection GetCatalogItems(bool recursive, bool useHttpClient, bool target)
        {
            NCatalogItemCollection catalog = new NCatalogItemCollection();
            // Retrieve the full list of catalog items from SRS
            CatalogItem[] catalogItems = null;
            if (target)
            {
                //Get Target SRS
                if (useHttpClient)
                {
                    catalogItems = this.ReportingTargetServiceClient.ListChildren(@"/", recursive);
                }
                else
                {
                    catalogItems = this.ReportingTargetServiceNonHttpClient.ListChildren(@"/", recursive);
                }
            }
            else
            {
                //Get Source SRS
                if (useHttpClient)
                {
                    catalogItems = this.ReportingSourceServiceClient.ListChildren(@"/", recursive);
                }
                else
                {
                    catalogItems = this.ReportingSourceServiceNonHttpClient.ListChildren(@"/", recursive);
                }
            }

            foreach (CatalogItem item in catalogItems)
            {
                NCatalogItem nItem = NCatalogItem.CreateNRCatalogItem(item);
                catalog.Add(nItem);
            }

            return catalog;
        }
        #endregion

        #endregion

        #region Private Helper Methods
        public BOConfiguredDataSourceCollection ConfiguredDataSourceCollection
        {
            get
            {
                if (_configuredDatasources == null)
                {
                    _configuredDatasources = new BOConfiguredDataSourceCollection();
                    _configuredDatasources.Fill();
                }

                return _configuredDatasources;
            }
        }
        public string LookupItemType(string itemPath, bool useHttpClient)
        {
            ItemTypeEnum itemType = ItemTypeEnum.Unknown;
            if (useHttpClient)
            {
                itemType = this.ReportingServiceClient.GetItemType(itemPath);
            }
            else
            {
                itemType = this.ReportingServiceNonHttpClient.GetItemType(itemPath);
            }

            string lookupType = Constants.CatalogItemTypes.None;
            switch (itemType)
            {
                case ItemTypeEnum.DataSource:
                    lookupType = Constants.CatalogItemTypes.DataSource;
                    break;
                case ItemTypeEnum.Folder:
                    lookupType = Constants.CatalogItemTypes.Folder;
                    break;
                case ItemTypeEnum.LinkedReport:
                    lookupType = Constants.CatalogItemTypes.LinkedReport;
                    break;
                case ItemTypeEnum.Report:
                    lookupType = Constants.CatalogItemTypes.Report;
                    break;                
            }
            return lookupType;
        }

        public string LookupReportLink(string itemPath, bool useHttpClient)
        {
            if (useHttpClient)
            {
                return this.ReportingServiceClient.GetReportLink(itemPath);
            }
            else
            {
                return this.ReportingServiceNonHttpClient.GetReportLink(itemPath);
            }
        }

        /// <summary>
        /// Build the CatalogItemCollection structure from SRS
        /// </summary>
        public NCatalogItemCollection LookupCatalogItems(string itemPath, bool recursive, bool useHttpClient, ReportingServiceClient srsClient)
        {
            NCatalogItemCollection catalogItemCollection = new NCatalogItemCollection();

            // If no ItemPath is specified, use the Root
            if (itemPath == null)
            {
                itemPath = "/";
            }

            // Retrieve the full list of catalog items from SRS
            CatalogItem[] unsecureCatalogItems = null;
            if (srsClient != null)
            {
                unsecureCatalogItems = srsClient.ListChildren(itemPath, recursive);
            }
            else
            {
                if (useHttpClient)
                {
                    unsecureCatalogItems = this.ReportingServiceClient.ListChildren(itemPath, recursive);
                }
                else
                {
                    unsecureCatalogItems = this.ReportingServiceNonHttpClient.ListChildren(itemPath, recursive);
                }
            }

            // Secure the list
            List<CatalogItem> secureCatalogList = buildSecureCatalogList(unsecureCatalogItems);
            foreach (CatalogItem item in secureCatalogList)
            {
                NCatalogItem nItem = NCatalogItem.CreateNRCatalogItem(item);
                
                // TODO: Still not decided if it's wise to get all the parameters upon login
                // Supply the parameters
                /*
                if (nItem.ItemType.Equals(Constants.CatalogItemTypes.Report))
                {
                    try
                    {
                        nItem.ReportParameters = GetReportParameters(nItem.Path, null);
                    }
                    catch
                    {
                        //TODO: Prepare an error display for this
                        //Error retrieving the parameters.                         
                        nItem.ReportParameters = null;
                    }
                }
                */
                catalogItemCollection.Add(nItem);
            }

            /*
            // Create a map between SRS folders and Nodes collections in the tree
            Hashtable folderMapTable = new Hashtable();

            if (itemPath == "/")
            {
                folderMapTable.Add("", catalogItemCollection);
            }
            else
            {
                folderMapTable.Add(itemPath, catalogItemCollection);
            }

            foreach (CatalogItem item in secureCatalogList)
            {
                // Get the path to the current item's folder
                string folderPath = item.Path.Substring(0, item.Path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase));
                // Get the node collection associated with that folder path
                NCatalogItemCollection nodes = folderMapTable[folderPath] as NCatalogItemCollection;
                if (nodes != null)
                {
                    NCatalogItem nci = NCatalogItem.CreateNRCatalogItem(item);
                    nodes.Add(nci);

                    if (nci.ItemType == Constants.CatalogItemTypes.Folder)
                    {
                        if (folderMapTable.ContainsKey(nci.Path))
                        {
                            throw new InvalidCatalogStructureException("Folder already exists");
                        }

                        NFolder nf = nci as NFolder;
                        folderMapTable.Add(nf.Path, nf.ChildCatalogItems);
                    }
                }
            }
            */
            return catalogItemCollection;
            
        }

        //TODO: Add the parameter values used in subscriptions
        public NReportParameterCollection GetReportParameters(string itemPath, ParameterValue[] parameterValueArray, ReportingServiceClient srsClient)
        {
            NReportParameterCollection parameters = new NReportParameterCollection();
            ReportParameter[] reportParameterArray;

            if (srsClient != null)
            {
                reportParameterArray = srsClient.GetReportParameters(itemPath,
                       null,
                       true,
                       parameterValueArray,
                       null);
            }
            else
            {
                reportParameterArray = ReportingServiceClient.GetReportParameters(itemPath,
                        null,
                        true,
                        parameterValueArray,
                        null);
            }
            foreach (ReportParameter parameter in reportParameterArray)
            {
                parameters.Add(new NReportParameter(parameter));                
            }

            return parameters;
        }

        private List<CatalogItem> buildSecureCatalogList(CatalogItem[] unsecureCatalogItems)
        {
            List<CatalogItem> secureCatalogList = new List<CatalogItem>(unsecureCatalogItems.Length);
            for (int checkNdx = unsecureCatalogItems.Length; checkNdx > 0; checkNdx--)
            {
                CatalogItem ci = unsecureCatalogItems[checkNdx - 1];

                string folderPath = string.Empty;

                if (ci.Type == ItemTypeEnum.Folder)
                {
                    folderPath = ci.Path;
                }
                else
                {
                    int lastSlash = ci.Path.LastIndexOf('/');
                    folderPath = ci.Path.Substring(0, lastSlash);
                }

                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (_authenticatedUserManager.CheckItemAccess(folderPath) == AccessStateType.Allowed)
                    {
                        secureCatalogList.Insert(0, ci);
                    }
                }
            }
            return secureCatalogList;
        }

        private void addDependencies(NCatalogItemCollection dependencyList, string catalogItemPath, string itemType, NCatalogItemCollection childCatalogItemList)
        {
            switch (itemType)
            {
                case Constants.CatalogItemTypes.DataSource:
                    addDependenciesForDataSource(dependencyList, catalogItemPath, childCatalogItemList);
                    break;
                case Constants.CatalogItemTypes.Report:
                    addDependenciesForReport(dependencyList, catalogItemPath, childCatalogItemList);
                    break;
                case Constants.CatalogItemTypes.Folder:
                    addDependenciesForFolder(dependencyList, catalogItemPath);
                    break;
            }
        }

        private void addDependenciesForFolder(NCatalogItemCollection dependencyList, string path)
        {
            NCatalogItemCollection folderCatalogItems = LookupCatalogItems(path, true, true, null);
            foreach (NCatalogItem catalogItem in folderCatalogItems)
            {
                bool interestingType = false;

                switch (catalogItem.ItemType)
                {
                    case Constants.CatalogItemTypes.MSAccess:
                    case Constants.CatalogItemTypes.MSExcel:
                    case Constants.CatalogItemTypes.LinkedReport:
                    case Constants.CatalogItemTypes.Report:
                    case Constants.CatalogItemTypes.DataSource:
                        interestingType = true;
                        break;
                }

                if (interestingType)
                {
                    // This item is referenced
                    addUniqueItemToDependencyList(dependencyList, catalogItem);

                    // Recurse through this item looking for any other dependencies
                    NFolder nf = catalogItem as NFolder;
                    if (nf != null)
                    {
                        addDependencies(dependencyList, nf.Path, nf.ItemType, nf.ChildCatalogItems);
                    }
                }
            }
        }

        private void addDependenciesForReport(NCatalogItemCollection dependencyList, string path, NCatalogItemCollection childCatalogItemList)
        {
            // Walk through the catalog looking for linked reports
            foreach (NCatalogItem catalogItem in childCatalogItemList)
            {
                if (catalogItem.ItemType == Constants.CatalogItemTypes.LinkedReport)
                {
                    string sourcePath = this.ReportingServiceClient.GetReportLink(catalogItem.Path);

                    if (sourcePath == path)
                    {
                        addUniqueItemToDependencyList(dependencyList, catalogItem);
                    }
                }
                else if (catalogItem.ItemType == Constants.CatalogItemTypes.Folder)
                {
                    // Recurse through this item looking for any other dependencies
                    NFolder nf = catalogItem as NFolder;
                    if (nf != null)
                    {
                        addDependenciesForReport(dependencyList, path, nf.ChildCatalogItems);
                    }
                }
            }
        }

        private void addDependenciesForDataSource(NCatalogItemCollection dependencyList, string path, NCatalogItemCollection childCatalogItemList)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Step through the catalog to see if the datasource it uses matches the path
                foreach (NCatalogItem catalogItem in childCatalogItemList)
                {
                    string dataSourcePath = string.Empty;

                    try
                    {
                        if (catalogItem.ItemType == Constants.CatalogItemTypes.Report)
                        {
                            // Get all DataSources referenced by this Report
                            DataSource[] referencedDataSourceList = this.ReportingServiceClient.GetItemDataSources(catalogItem.Path);
                            foreach (DataSource dataSource in referencedDataSourceList)
                            {
                                // We only care about DataSourceReference types, otherwise the 
                                // DataSource isn't defined as an object in the catalog, it is 
                                // defined inside the Report itself.
                                DataSourceReference dataSourceReference = dataSource.Item as DataSourceReference;
                                if (dataSourceReference != null)
                                {
                                    dataSourcePath = dataSourceReference.Reference;
                                }
                            }
                        }
                        else if (catalogItem.ItemType == Constants.CatalogItemTypes.Folder)
                        {
                            // Recurse through this item looking for any other dependencies
                            NFolder nf = catalogItem as NFolder;
                            if (nf != null)
                            {
                                addDependenciesForDataSource(dependencyList, path, nf.ChildCatalogItems);
                            }
                        }
                    }
                    catch //( Exception ex )
                    {
                        // Something failed when trying to retrieve the DataSource path for this 
                        // catalog item don't stop looking, but this item will not be included in 
                        // the link list
                        //Debug.WriteLine( ex.Message );
                    }

                    if (dataSourcePath == path)
                    {
                        // This item is referenced
                        addUniqueItemToDependencyList(dependencyList, catalogItem);

                        // Recurse through this item looking for any other dependencies
                        addDependencies(dependencyList, catalogItem.Path, catalogItem.ItemType, childCatalogItemList);
                    }
                }
            }
        }

        private static void addUniqueItemToDependencyList(NCatalogItemCollection dependencyList, NCatalogItem catalogItem)
        {
            bool isUnique = true;
            foreach (NCatalogItem ci in dependencyList)
            {
                if (ci.Path == catalogItem.Path)
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                dependencyList.Add(catalogItem);
            }
        }

        #endregion  
    }
}
