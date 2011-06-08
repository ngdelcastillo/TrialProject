using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navitaire.Reporting.Services.Authentication;
using Navitaire.Reporting.Services.BusinessObjects;
using Navitaire.Reporting.Services;
using Navitaire.Reporting.Security;
using System.Collections.Specialized;
using Navitaire.Reporting.Services.Catalog;
using Navitaire.Reporting.Services.SRS;

namespace Navitaire.Reporting.Web.Models
{
    public class SettingsModel
    {
        #region Declarations
        
        private StringCollection _reportStoredProcedures;
        public static string ReportPath { get; set; }
        public static string FolderPath { get; set; }
        public static string State { get; set; }
        
        #endregion
        #region Properties
        public BOExportMapCollection ExportMappings { get; set; }
        public BOConfigurationCollection ApplicationConfigurations { get; set; }
        public BOConfigurationCollection SubscriptionConfigurations { get; set; }
        public Dictionary<int, string> SubscriptionLogLevel { get; set; }
        public BOConfiguredRoleCollection ConfiguredRoles { get; set; }
        public StringCollection Roles { get; set; }
        public StringCollection ExportMapDisplayOptions { get; set; }
        public CatalogModel Catalog { get; set; }
        public List<string> SubscriptionNameFormat { get; set; }

        public BOUserAccessCollection UserCollection { get; set; }
        public BORoleAccessCollection RoleCollection { get; set; }

        public StringCollection AllowedRoleCollection { get; set; }
        public StringCollection DeniedRoleCollection { get; set; }


        public StringCollection ReportStoredProcedures
        {
            get { return _reportStoredProcedures; }
            set { _reportStoredProcedures = value; }
        }
        #endregion

        

        #region Get methods
        public static SettingsModel GetConfigurations(AuthenticatedUserManager user)
        {
            BOConfigurationCollection collection = user.ConfigurationCollection;
            collection.Fill();
            SettingsModel model = new SettingsModel();

            model.ApplicationConfigurations = new BOConfigurationCollection();
            
            #region appSettings value assignments
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.StartupPageAddress });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.SMTPServer });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.SupportEmail });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.BaseUrl });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.MinimumUIYear });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.SRSBaseUrl });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.SRSReportExecutionPage });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.SRSReportExecutionServicePage });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.SRSReportServicePage });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.FTPServer });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.FTPPath });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.FTPUser });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.FTPPassword });
            model.ApplicationConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableApplicationSettings.CSVDelimiter });
            #endregion
            
            model.SubscriptionConfigurations = new BOConfigurationCollection();

            #region subscriptionSettings value assignments
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.ScheduleServiceSignature });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.TimerQueuePollingInterval });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.TimerQueueLoadingInterval });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.TimerMaintenancePollingInterval });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.MaxScheduleThreads });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.HeadquartersTimeZone });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.SubscriptionLogLevel });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.SubscriptionNameFormat });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.MaxReportFileAge });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.MaxScheduleFailCount });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.MaxScheduleQueueAge });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.SubscriptionReportTimeout });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.SubscriptionZipThreshold });
            model.SubscriptionConfigurations.Add(new BOConfiguration() { PropertyName = Constants.AvailableSubscriptionSettings.SubscriptionEmailAttachmentThreshold });
            #endregion

            foreach (BOConfiguration setting in collection)
            {
                if (model.ApplicationConfigurations[setting.PropertyName] != null)
                {
                    model.ApplicationConfigurations[setting.PropertyName].PropertyValue = setting.PropertyValue;
                    model.ApplicationConfigurations[setting.PropertyName].PropertyDescription = setting.PropertyDescription;
                }
            }
            foreach (BOConfiguration setting in collection)
            {
                if (model.SubscriptionConfigurations[setting.PropertyName] != null)
                {
                    model.SubscriptionConfigurations[setting.PropertyName].PropertyValue = setting.PropertyValue;
                    model.SubscriptionConfigurations[setting.PropertyName].PropertyDescription = setting.PropertyDescription;

                }
            }
          
            model.SubscriptionNameFormat = new List<string>()
            {
                Constants.SubscriptionNameFormats.NameDate,
                Constants.SubscriptionNameFormats.ReportDate,
                Constants.SubscriptionNameFormats.ReportNameDate
            };

            model.SubscriptionLogLevel = new Dictionary<int, string>()
            {
                {(int)TraceLevel.None, TraceLevel.None.ToString()},
                {(int)TraceLevel.Standard, TraceLevel.Standard.ToString()},
                {(int)TraceLevel.Verbose, TraceLevel.Verbose.ToString()}
            };
            return model;
        }
       

        public static SettingsModel GetRoles(AuthenticatedUserManager user)
        {
            user.ConfiguredRoleCollection.Fill();
            SettingsModel model = new SettingsModel();
            model.ConfiguredRoles = user.ConfiguredRoleCollection;
            model.Roles = new StringCollection();
            model.Roles = AuthenticationManager.Authenticate(user.Username, user.Password, user.Domain).SecurityRoleCollection;
            foreach (BOConfiguredRole role in model.ConfiguredRoles)
            {
                if(model.Roles.Contains(role.RoleName))
                {
                    model.Roles.Remove(role.RoleName);
                }
            }
            return model;
        }

        public static SettingsModel GetExportMappings(AuthenticatedUserManager user)
        {
            SettingsModel model = InitializeCatalog(user, CatalogModel.SRSAuthenticate(user));
           
            CatalogModel proxyCatalog = new CatalogModel();
            model.Catalog.Catalogs.RemoveAll(item => item.ItemType != "Folder" && item.ItemType !="Report");
            foreach (CatalogModel catModel in model.Catalog.Catalogs)
            {
                catModel.Children.RemoveAll(item => item.ItemType != "Folder" && item.ItemType != "Report");
                foreach (CatalogModel catModel2 in catModel.Children)
                {
                    catModel2.Children.RemoveAll(item => item.ItemType != "Folder" && item.ItemType != "Report");
                }
            }
            model.Catalog.Catalogs.RemoveAll(item => item.ItemType == "Folder" && item.Children.Count == 0);

            foreach (CatalogModel catModel in model.Catalog.Catalogs)
            {
                catModel.Catalogs.RemoveAll(item => item.ItemType == "Folder" && item.Children.Count == 0);
                foreach (CatalogModel catModel2 in model.Catalog.Catalogs)
                {
                    catModel2.Catalogs.RemoveAll(item => item.ItemType == "Folder" && item.Children.Count == 0);
                }
            }
            model.ExportMappings = new BOExportMapCollection();
            model.ExportMappings.Fill();

            model.ExportMapDisplayOptions = new StringCollection();
            model.ExportMapDisplayOptions.Add(Constants.ExportMapCatalogOptions.ExportAndView);
            model.ExportMapDisplayOptions.Add(Constants.ExportMapCatalogOptions.ExportOnly);
            return model;
        }

        

        public static SettingsModel GetSecurityCatalogItems(AuthenticatedUserManager user, string path)
        {
            SettingsModel model = InitializeCatalog(user, CatalogModel.SRSAuthenticate(user));
            model.Catalog.AllItems.RemoveAll(item => item.ItemType != "Folder");
            model.Catalog.Catalogs.RemoveAll(item => item.ItemType != "Folder");
            model.AllowedRoleCollection = new StringCollection();
            model.DeniedRoleCollection = new StringCollection();
            foreach (CatalogModel catModel in model.Catalog.Catalogs)
            {
                catModel.Children.RemoveAll(item => item.ItemType != "Folder");
                foreach (CatalogModel catModel2 in catModel.Children)
                {
                    catModel2.Children.RemoveAll(item => item.ItemType != "Folder");
                }
            }
            
            if (path != string.Empty)
            {
                user.ConfiguredRoleCollection.Fill();
                user.UserAccessCollection.Fill();
                user.RoleAccessCollection.Fill();

                foreach (BOConfiguredRole securityRole in user.ConfiguredRoleCollection)
                {
                    BORoleAccess roleAccess = user.RoleAccessCollection[securityRole.RoleName, path];
                    if (roleAccess != null)
                    {
                        if (roleAccess.AccessState == AccessStateType.Allowed)
                        {
                            model.AllowedRoleCollection.Add(securityRole.RoleName);
                        }
                        else
                        {
                            model.DeniedRoleCollection.Add(securityRole.RoleName);
                        }
                    }
                    else
                    {
                        model.DeniedRoleCollection.Add(securityRole.RoleName);
                    }
                }
               

                model.UserCollection = user.UserAccessCollection.FilterForItemPath(path);
                model.RoleCollection = user.RoleAccessCollection.FilterForItemPath(path);
            }
            return model;
        }

        public void FillReportStoredProcedures(AuthenticatedUserManager user, string reportPath)
        {
            ExportMapManager exportMapManager = new ExportMapManager(user);
            _reportStoredProcedures = exportMapManager.LookupStoredProcFromRdl(reportPath);
        }


        private static SettingsModel InitializeCatalog(AuthenticatedUserManager user, ReportingServiceClient proxy)
        {
            SettingsModel model = new SettingsModel();
            model.Catalog = new CatalogModel();
            model.Catalog = CatalogModel.GetCatalogItems(user, proxy);
            return model;
        }
        #endregion

        public static SettingsModel SaveConfigurations(Dictionary<string, string> configurations, AuthenticatedUserManager user)
        {
            BOConfigurationCollection origConfigurations = user.ConfigurationCollection;
            origConfigurations.Fill();
            foreach (string key in configurations.Keys)
            {
                BOConfiguration configuration = origConfigurations[key];
                if (configuration != null)
                {
                    configuration.PropertyName = key;
                    configuration.PropertyValue = configurations[key];
                    configuration.Save(user.AuthenticatedUserSignature);
                }
            }
            return GetConfigurations(user);
        }

        public static SettingsModel SaveConfiguredRoles(string role, string action, AuthenticatedUserManager user)
        {
            BOConfiguredRole configuredRole;
            
            if (user.ConfiguredRoleCollection == null)
            {
                user.ConfiguredRoleCollection.Fill();
            }

            configuredRole = user.ConfiguredRoleCollection[role];
            if (configuredRole == null)
            {
                configuredRole = new BOConfiguredRole();
                configuredRole.RoleName = role;
            }

            if (action == "AddRole")
            {
                configuredRole.Save(user.AuthenticatedUserSignature);
            }
            else
            {
                configuredRole.Delete();
            }
            return GetRoles(user);
        }

        public static SettingsModel SaveExportMapping(AuthenticatedUserManager user, string reportPath, string procName, string displayOption)
        {
            SettingsModel.ReportPath = string.Empty;
            ExportMapManager manager = new ExportMapManager(user);
            BOExportMap exportMap = new BOExportMap();
            exportMap.ProcName = procName;
            exportMap.ReportPath = reportPath;
            exportMap.DisplayOptions = displayOption;
            exportMap.DataSourcePath = manager.LookupDataSourceFromRdl(reportPath);
            exportMap.Save(user.AuthenticatedUserSignature);
            return SettingsModel.GetExportMappings(user);
        }

        public static SettingsModel DeleteExportMapping(AuthenticatedUserManager user, string reportPath)
        {
            SettingsModel.ReportPath = string.Empty;
            BOExportMap exportMap = new BOExportMap();
            ExportMapManager manager = new ExportMapManager(user);
            exportMap = manager.FindAssociatedExportMap(reportPath, false);
            exportMap.Delete();
            return SettingsModel.GetExportMappings(user);
        }
    }
}