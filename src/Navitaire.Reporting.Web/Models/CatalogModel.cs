using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Net;
using Navitaire.Reporting.Services;
using Navitaire.Reporting.Services.Catalog;
using Navitaire.Reporting.Services.Authentication;
using Navitaire.Reporting.Services.BusinessObjects;
using Navitaire.Reporting.Services.SRS;

namespace Navitaire.Reporting.Web.Models
{
    public class CatalogModel
    {
        #region Private Variables
        //private Exception _error = new Exception();
        private ReportingServiceClient _srsProxy;
        private List<CatalogModel> _catalogs = new List<CatalogModel>();
        private List<CatalogModel> _children = new List<CatalogModel>();
        private List<CatalogModel> _allItems = new List<CatalogModel>();

        private NReportParameterCollection _reportParameters = new NReportParameterCollection();
        private NameValueCollection _itemProperties = new NameValueCollection();
        private DataSourceItem _dataSourceItem = new DataSourceItem();
        private ReportProperty _reportItem = new ReportProperty();
        #endregion
        
        #region Properties       
        public string ImageUrlSuffix { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Parent { get; set; }
        public string Key { get; set; }
        public string Result { get; set; }
        public string PostMethod { get; set; }

        public Exception Error
        {
            //get { return _error; }
            //set { _error = value; }
            get;
            set;
        }
        public ReportingServiceClient SRSProxy
        {
            get
            {
                return _srsProxy;
            }
            set
            {
                _srsProxy = value;
            }
        }
        public List<CatalogModel> Catalogs
        {
            get
            {
                return _catalogs;
            }
            set
            {
                _catalogs = value;
            }
        }
        public List<CatalogModel> Children 
        { 
            get 
            { 
                return _children; 
            } 
            set 
            { 
                _children = value; 
            } 
        }
        public List<CatalogModel> AllItems
        {
            get
            {
                return _allItems;
            }
            set
            {
                _allItems = value;
            }
        }
        public NReportParameterCollection ReportParameters
        {
            get
            {
                return _reportParameters;
            }
            set
            {
                _reportParameters = value;
            }
        }
        public NameValueCollection ItemProperties
        {
            get { return _itemProperties; }
            set { _itemProperties = value; }
        }
        public DataSourceItem DataSourceItem 
        {
            get { return _dataSourceItem; }
            set { _dataSourceItem = value; }
        }
        public ReportProperty ReportItem
        {
            get { return _reportItem; }
            set { _reportItem = value; }
        }
        #endregion

        /// <summary>
        /// Lets store the SRS Authid so that we wont need to relogin everytime we access SRS.
        /// This is used in web service in SRS
        /// </summary>
        public static ReportingServiceClient SRSAuthenticate(AuthenticatedUserManager user)
        {
            return user.CatalogInteractionManager.ReportingServiceClient;            
        }

        /// <summary>
        /// Gets the catalog items from the SRS.
        /// </summary>
        public static CatalogModel GetCatalogItems(AuthenticatedUserManager user, ReportingServiceClient srsProxy)
        {
            CatalogModel final = new CatalogModel();
            NCatalogItemCollection catalogs = user.CatalogInteractionManager.LookupCatalogItems(null, true, true, srsProxy);
            List<CatalogModel> models = new List<CatalogModel>();            

            foreach (NCatalogItem item in catalogs)
            {
                models.Add(ToModel(item));                
            }
            final.AllItems = models;

            //Process Child
            List<CatalogModel> finalCatalog = new List<CatalogModel>();            
            foreach (var parent in models)
            {
                parent.AllItems.Add(parent);
                if (parent.ItemType.Equals("Folder", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var child in models)
                    {
                        //check parents
                        if (child.Parent.Equals(parent.Path, StringComparison.OrdinalIgnoreCase))
                        {
                            parent.Children.Add(child);
                        }
                    }

                    if(parent.Parent.Equals("root", StringComparison.OrdinalIgnoreCase))
                    {
                        finalCatalog.Add(parent);
                    }
                }                
            }
            final.Catalogs = finalCatalog;
            final.SRSProxy = srsProxy;
            return final;
        }

        /// <summary>
        /// Converts NCatalogItem to CatalogModel
        /// </summary>
        public static CatalogModel ToModel(NCatalogItem item)
        {
            CatalogModel model = new CatalogModel();
            //Construct the key
            model.Key = item.Path.Replace("/", "_");
            model.Key = model.Key.Replace(" ", "");
            
            model.Name = item.Name;
            model.Path = "root"+ item.Path;
            model.ItemType = item.ItemType;
            model.ImageUrlSuffix = item.ImageUrlSuffix;
            model.ReportParameters = item.ReportParameters;

            string parent = "root" + item.Path;
            string[] nodes = parent.Split('/');
            int length = nodes.Length;
            int index = 1;
            parent = nodes[0];
            while(index < length-1)
            {
                parent += "/" + nodes[index];
                index ++;
            }
            model.Parent = parent;
            return model;
        }

        /// <summary>
        /// Gets the content of the selected node in the Catalog tree.
        /// </summary>
        public static CatalogModel GetItem(List<CatalogModel> allItems, string key)
        {
            foreach(CatalogModel item in allItems)
            {           
                if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }                
            }
            return null;
        }

        /// <summary>
        /// Gets the Datasource from the SRS.
        /// </summary>
        public void GetDataSourceItem(AuthenticatedUserManager user, ReportingServiceClient srsProxy)
        {
            string address = Path.Substring(Path.IndexOf('/'));
            NDatasource datasource = user.CatalogInteractionManager.LookupDataSource(address, srsProxy);
            if (datasource != null)
            {
                DataSourceItem.itemProperties.Clear();

                DataSourceItem.ConnectionString = datasource.ConnectionString;
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Datasource, CatalogConstants.DataSourceLabels.DatasourceLabel, datasource.Name));
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Database, CatalogConstants.DataSourceLabels.DatabaseLabel, datasource.DatabaseName));
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Server, CatalogConstants.DataSourceLabels.ServerLabel, datasource.ServerName));
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Provider, CatalogConstants.DataSourceLabels.ProviderLabel, datasource.Provider));
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.User, CatalogConstants.DataSourceLabels.UserNameLabel, datasource.UserName));
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Password, CatalogConstants.DataSourceLabels.Password, ""));
                DataSourceItem.itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.ConfirmPassword, CatalogConstants.DataSourceLabels.ConfirmPassLabel, ""));
            }
            else
            {
                //Throw an error
                Error = new Exception(string.Format("No datasource property found in this path {0}.", address));
            }            
        }

        /// <summary>
        /// Gets all of the Report Parameters from the SRS.
        /// </summary>
        public void GetReportParameters(AuthenticatedUserManager user, ReportingServiceClient srsClient)
        {
            string address = Path.Substring(Path.IndexOf('/'));
            ReportParameters = user.CatalogInteractionManager.GetReportParameters(address, null, srsClient);
        }

        /// <summary>
        /// Gets the information of the catalog item
        /// </summary>
        public void GetItemProperty(AuthenticatedUserManager user)
        {
            string address = Path.Substring(Path.IndexOf('/'));
            ItemProperties = user.CatalogInteractionManager.LookupItemProperties(address);
        }

        /// <summary>
        /// Deletes the catalog item
        /// </summary>        
        public void DeleteItem(AuthenticatedUserManager user)
        {
            string address = Path.Substring(Path.IndexOf('/'));
            bool supressError = false;
            user.CatalogInteractionManager.DeleteCatalogItem(address, supressError, true);            
        }

        /// <summary>
        /// Runs the report in the SRS Web Service.
        /// Inherits the host of the request.
        /// </summary>
        public void RunReport(string host)
        {
            try
            {
                //string encryptedPath = HttpUtility.UrlEncode(Utility.EncryptValue(Path));
                string itemPath = Path.Substring(Path.IndexOf('/'));
                StringBuilder paramBuilder = new StringBuilder();
                paramBuilder.AppendFormat("&rs:Command=Render&rc:toolbar=true&rs:Format=html4.0&rc:parameters=false");
                foreach (NReportParameter parameter in ReportParameters)
                {
                    if (!parameter.Name.Equals("ProcVersion", StringComparison.OrdinalIgnoreCase))
                    {
                        //Include only with selected values or 
                        if ((!string.IsNullOrEmpty(parameter.SelectedValue)) || parameter.ValidValues.Count < 1)
                        {
                            paramBuilder.AppendFormat("&{0}={1}",
                                parameter.Name,
                                parameter.SelectedValue);
                        }
                    }
                }

                string srsReportExecutionUrl = GetUrl(host);
                Result = string.Format("{0}?{1}{2}", srsReportExecutionUrl, itemPath, paramBuilder.ToString());                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save the datasource in the SSRS.
        /// </summary>
        public void SaveDataSource(AuthenticatedUserManager user, string address)
        {
            //TODO: Need to add validation for each textboxes                        
            string datasourceName = string.Empty;
            string databaseName = string.Empty;
            string serverName = string.Empty;
            string userName = string.Empty; 
            string password = string.Empty;
            string confirmPassword = string.Empty;
            bool canSave = true;

            foreach (var item in DataSourceItem.itemProperties)
            {
                switch (item.PropertyName)
                {
                    case "DatasourceName":
                        datasourceName = item.Value;
                        break;

                    case "DatabaseName":
                        databaseName = item.Value;
                        break;

                    case "ServerName":
                        serverName = item.Value;
                        break;

                    case "UserName":
                        userName = item.Value;
                        break;

                    case "Password":
                        password = item.Value;
                        break;

                    case "ConfirmPassword":
                        confirmPassword = item.Value;
                        break;
                }
            }
            //TODO: Add Validation in this part
            if (canSave)
            {                
                user.CatalogInteractionManager.CreateDataSource(
                    datasourceName,
                    address,
                    "SQL",
                    serverName,
                    databaseName,
                    userName,
                    password,
                    confirmPassword,
                    true);                
            }
            else
            {
                //Alert message: Incomplete information
            }
        }

        /// <summary>
        /// Add the Report RDL in the SRS Web Service.        
        /// </summary>
        public void AddReport()
        {


        }

        /// <summary>
        /// Gets the SRSUrl used in SRS Web Service.
        /// Inherits the host of the request.
        /// </summary>
        private string GetUrl(string host)
        {
            BOConfigurationCollection configurationCollection = new BOConfigurationCollection();
            configurationCollection.Fill();

            string srsBaseUrl = configurationCollection[Constants.AvailableApplicationSettings.SRSBaseUrl].PropertyValue.TrimEnd(new char[] { '/' });
            string head = srsBaseUrl.Substring(0, srsBaseUrl.Length - srsBaseUrl.Substring(srsBaseUrl.IndexOf("//")).Length);
            srsBaseUrl = srsBaseUrl.Substring(srsBaseUrl.IndexOf("//"));
            string extension = srsBaseUrl.Substring(srsBaseUrl.IndexOf(':'));
            srsBaseUrl = head + "//" + host + extension;

            string srsReportExecutionPage = configurationCollection[Constants.AvailableApplicationSettings.SRSReportExecutionPage].PropertyValue;
            return string.Format("{0}/{1}", srsBaseUrl, srsReportExecutionPage);
        }
    }

    public class DataSourceItem
    {
        private Collection<DataSourceProperty> _itemProperties = new Collection<DataSourceProperty>();
        
        public string ConnectionString { get; set; }
        public Collection<DataSourceProperty> itemProperties 
        {
            get { return _itemProperties; }
            set { _itemProperties = value; }
        }

        //Upon Initialization, create the properties inside the collection
        public DataSourceItem()
        {
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Datasource, CatalogConstants.DataSourceLabels.DatasourceLabel, string.Empty));
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Database, CatalogConstants.DataSourceLabels.DatabaseLabel, string.Empty));
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Server, CatalogConstants.DataSourceLabels.ServerLabel, string.Empty));
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Provider, CatalogConstants.DataSourceLabels.ProviderLabel, string.Empty));
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.User, CatalogConstants.DataSourceLabels.UserNameLabel, string.Empty));
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.Password, CatalogConstants.DataSourceLabels.Password, string.Empty));
            itemProperties.Add(new DataSourceProperty(CatalogConstants.DataSourceProperties.ConfirmPassword, CatalogConstants.DataSourceLabels.ConfirmPassLabel, string.Empty));
        }
    }

    public class DataSourceProperty
    {
        public string PropertyName { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }

        public DataSourceProperty(string propertyName, string label, string value)
        {
            PropertyName = propertyName;
            Label = label;
            Value = value;
        }
    }

    public class ReportProperty
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
    }

    //Constants for Catalog Model
    public static class CatalogConstants
    {
        public struct DataSourceLabels
        {
            public const string DatasourceLabel = "Data Source Name:";
            public const string DatabaseLabel = "Database Name:";
            public const string ServerLabel = "Server Name:";
            public const string ProviderLabel = "Provider:";
            public const string UserNameLabel = "User Name:";
            public const string Password = "Password:";
            public const string ConfirmPassLabel = "Confirm Password:";
        }

        public struct DataSourceProperties
        {
            public const string Datasource = "DatasourceName";
            public const string Database = "DatabaseName";
            public const string Server = "ServerName";
            public const string Provider = "Provider";
            public const string User = "UserName";
            public const string Password = "Password";
            public const string ConfirmPassword = "ConfirmPassword";
        }
    }
}