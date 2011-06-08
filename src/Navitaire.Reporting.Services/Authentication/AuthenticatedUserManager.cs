using System;
using System.Collections.Specialized;
using System.Globalization;

using Navitaire.Reporting.Security;
using Navitaire.Reporting.Services.SRS;
using Navitaire.Reporting.Services.Help;
using Navitaire.Reporting.Services.BusinessObjects;
using Navitaire.Reporting.Services.Catalog;
namespace Navitaire.Reporting.Services.Authentication
{
    public class AuthenticatedUserManager : IDisposable
    {
        #region Private Variables
        private long _ncaUserId = -1;        
        private DateTime _ncaExpiration = DateTime.MinValue;
        private PermissionCollection _permissionCollection;        
        private ActionHelpMapTopicCollection _actionHelpMapTopicCollection;
        private CatalogInteractionManager _catalogInteractionManager;

        //Business Objects
        private BOConfigurationCollection _configurationCollection;
        private BOConfiguredRoleCollection _configuredRoleCollection;        
        private BORoleAccessCollection _roleAccessCollection;
        private BOUserAccessCollection _userAccessCollection;

        //--SRS
        private ExecutionServiceClient _executionServiceClient;
        private ReportingServiceClient _reportingServiceClient;
        private ReportingServiceNonHttpClient _reportingServiceNonHttpClient;
        private ReportingServiceClient _reportingTargetServiceClient;
        private ReportingServiceNonHttpClient _reportingTargetServiceNonHttpClient;
        private ReportingServiceClient _reportingSourceServiceClient;
        private ReportingServiceNonHttpClient _reportingSourceServiceNonHttpClient;

        #endregion

        #region Properties
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }        
        public string NcaRoleCode { get; set; }
        public StringCollection SecurityRoleCollection { get; set; }

        //Used in Deploy only
        public string TargetSRSUrl { get; set; }
        public string SourceSRSUrl { get; set; }

        public ActionHelpMapTopicCollection ActionHelpMapTopicCollection
        {
            get
            {
                if (_actionHelpMapTopicCollection == null)
                {
                    _actionHelpMapTopicCollection = new ActionHelpMapTopicCollection();
                }

                return _actionHelpMapTopicCollection;
            }
        }

        public CatalogInteractionManager CatalogInteractionManager
        {
            get
            {
                if (_catalogInteractionManager == null)
                {
                    _catalogInteractionManager = new CatalogInteractionManager(this);
                }

                return _catalogInteractionManager;
            }
        }

        public string AuthenticatedUserSignature
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", Domain, Username);
            }
        }

        #endregion

        #region BOProperties
        public BOConfigurationCollection ConfigurationCollection
        {
            get
            {
                if (_configurationCollection == null)
                {
                    _configurationCollection = new BOConfigurationCollection();
                    _configurationCollection.Fill();
                }

                return _configurationCollection;
            }
        }

        public BOConfiguredRoleCollection ConfiguredRoleCollection
        {
            get
            {
                if (_configuredRoleCollection == null)
                {
                    _configuredRoleCollection = new BOConfiguredRoleCollection();
                    _configuredRoleCollection.Fill();
                }

                return _configuredRoleCollection;
            }
        }
      
        public BORoleAccessCollection RoleAccessCollection
        {
            get
            {
                if (_roleAccessCollection == null)
                {
                    _roleAccessCollection = new BORoleAccessCollection();
                    _roleAccessCollection.Fill();
                }

                return _roleAccessCollection;
            }
        }


        public BOUserAccessCollection UserAccessCollection
        {
            get
            {
                if (_userAccessCollection == null)
                {
                    _userAccessCollection = new BOUserAccessCollection();
                    _userAccessCollection.Fill();
                }
                return _userAccessCollection;
            }
        }
        #endregion

        #region Internal Properties
        internal ExecutionServiceClient ExecutionServiceClient
        {
            get
            {
                if (_executionServiceClient == null)
                {
                    string srsBaseUrl = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSBaseUrl].PropertyValue.TrimEnd(new char[] { '/' });
                    string srsReportExecutionServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportExecutionServicePage].PropertyValue;
                    string executionServiceUrl = string.Format(CultureInfo.InvariantCulture,
                        "{0}/{1}",
                        srsBaseUrl,
                        srsReportExecutionServicePage);

                    _executionServiceClient = new ExecutionServiceClient(Username, Password, Domain, executionServiceUrl);
                    _executionServiceClient.LogonUser(Username, Password, Domain);
                }

                return _executionServiceClient;
            }
        }

        internal ReportingServiceClient ReportingServiceClient
        {
            get
            {
                checkClientCacheExpired();

                if (_reportingServiceClient == null)
                {
                    string srsBaseUrl = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSBaseUrl].PropertyValue.TrimEnd(new char[] { '/' });
                    string srsReportServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportServicePage].PropertyValue;
                    string reportServiceUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", srsBaseUrl, srsReportServicePage);

                    // Create a new client connection
                    _reportingServiceClient = new ReportingServiceClient(Username, Password, Domain, reportServiceUrl);
                    _reportingServiceClient.LogonUser(Username, Password, Domain);
                    
                }

                return _reportingServiceClient;
            }
        }

        internal ReportingServiceNonHttpClient ReportingServiceNonHttpClient
        {
            get
            {
                if (_reportingServiceNonHttpClient == null)
                {
                    string srsBaseUrl = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSBaseUrl].PropertyValue.TrimEnd(new char[] { '/' });
                    string srsReportServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportServicePage].PropertyValue;
                    string reportServiceUrl = string.Format(CultureInfo.InvariantCulture,
                        "{0}/{1}",
                        srsBaseUrl,
                        srsReportServicePage);

                    // Create a new client connection
                    _reportingServiceNonHttpClient = new ReportingServiceNonHttpClient(Username, Password, Domain, reportServiceUrl);
                    _reportingServiceNonHttpClient.LogonUser(Username, Password, Domain);
                }

                return _reportingServiceNonHttpClient;
            }
        }

        internal ReportingServiceClient ReportingTargetServiceClient
        {
            get
            {
                if (_reportingTargetServiceClient == null)
                {
                    string srsTargetUrl = TargetSRSUrl;
                    string srsReportServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportServicePage].PropertyValue;
                    string reportTargetServiceUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", srsTargetUrl, srsReportServicePage);

                    // Create a new client connection
                    _reportingTargetServiceClient = new ReportingServiceClient(Username, Password, Domain, reportTargetServiceUrl);
                    _reportingTargetServiceClient.LogonUser(Username, Password, Domain);
                }

                return _reportingTargetServiceClient;
            }
        }

        internal ReportingServiceNonHttpClient ReportingTargetServiceNonHttpClient
        {
            get
            {
                if (_reportingTargetServiceNonHttpClient == null)
                {
                    string srsTargetUrl = TargetSRSUrl;
                    string srsReportServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportServicePage].PropertyValue;
                    string reportTargetServiceUrl = string.Format(CultureInfo.InvariantCulture,
                        "{0}/{1}",
                        srsTargetUrl,
                        srsReportServicePage);

                    // Create a new client connection
                    _reportingTargetServiceNonHttpClient = new ReportingServiceNonHttpClient(Username, Password, Domain, reportTargetServiceUrl);
                    _reportingTargetServiceNonHttpClient.LogonUser(Username, Password, Domain);
                }

                return _reportingTargetServiceNonHttpClient;
            }
        }

        internal ReportingServiceClient ReportingSourceServiceClient
        {
            get
            {
                if (_reportingSourceServiceClient == null)
                {
                    string srsSourceUrl = SourceSRSUrl;
                    string srsReportServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportServicePage].PropertyValue;
                    string reportSourceServiceUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", srsSourceUrl, srsReportServicePage);

                    // Create a new client connection
                    _reportingSourceServiceClient = new ReportingServiceClient(Username, Password, Domain, reportSourceServiceUrl);
                    _reportingSourceServiceClient.LogonUser(Username, Password, Domain);
                }

                return _reportingSourceServiceClient;
            }
        }

        internal ReportingServiceNonHttpClient ReportingSourceServiceNonHttpClient
        {
            get
            {
                if (_reportingSourceServiceNonHttpClient == null)
                {
                    string srsSourceUrl = SourceSRSUrl;
                    string srsReportServicePage = this.ConfigurationCollection[Constants.AvailableApplicationSettings.SRSReportServicePage].PropertyValue;
                    string reportSourceServiceUrl = string.Format(CultureInfo.InvariantCulture,
                        "{0}/{1}",
                        srsSourceUrl,
                        srsReportServicePage);

                    // Create a new client connection
                    _reportingSourceServiceNonHttpClient = new ReportingServiceNonHttpClient(Username, Password, Domain, reportSourceServiceUrl);
                    _reportingSourceServiceNonHttpClient.LogonUser(Username, Password, Domain);
                }

                return _reportingSourceServiceNonHttpClient;
            }
        }
        #endregion

        #region Constructor
        public AuthenticatedUserManager()
        { }

        public AuthenticatedUserManager(string username, string password, string domain)
        {
            #region Validate Parameters

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("userName");
            }
            else if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }
            else if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }

            #endregion Validate Parameters
            Username = username;
            Password = password;
            Domain = domain;
        }
        #endregion

        #region Methods
        public static bool Authenticate(string username, string password, string domain)
        {
            bool login = false;
            try
            {
                AuthenticationManager.Authenticate(username, password, domain);
                login = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return login;
        }

        public void AuthenticationForApplication()
        {
            // Authenticate to the Security system
            AuthenticationInformation authInfo = AuthenticationManager.Authenticate(Username, Password, Domain);
            _ncaUserId = authInfo.UserId;
            _ncaExpiration = authInfo.UserAccountExpirationUtc;
            NcaRoleCode = authInfo.ActiveRoleCode;
            _permissionCollection = new PermissionCollection(authInfo.PermissionCollection);

            if (SecurityRoleCollection == null)
            {
                SecurityRoleCollection = new StringCollection();
            }
            SecurityRoleCollection.Clear();

            foreach (string role in authInfo.SecurityRoleCollection)
            {
                SecurityRoleCollection.Add(role);
            }
            synchronizeUserToDatabase();
        }

        

        public bool CheckUserPermission(string requiredPermission)
        {
            StringCollection requiredPermissions = new StringCollection();
            requiredPermissions.Add(requiredPermission);

            return CheckUserPermission(requiredPermissions);
        }

        private bool CheckUserPermission(StringCollection requiredPermissions)
        {
            bool allowed = false;
            try
            {
                if (Domain.Equals("sys", StringComparison.OrdinalIgnoreCase))
                {
                    allowed = true;
                }
                else
                {
                    foreach (string requiredPermission in requiredPermissions)
                    {
                        Permission authenticatedPermission = _permissionCollection[requiredPermission];
                        if (authenticatedPermission != null)
                        {
                            if(authenticatedPermission.Allowed)
                            {
                                allowed = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                allowed = false;
            }
            return allowed;
        }

        public AccessStateType CheckItemAccess(string itemPath)
        {
            #region Validate Parameters
            if (string.IsNullOrEmpty(itemPath))
            {
                throw new ArgumentNullException("itemPath");
            }
            #endregion Validate Parameters

            AccessStateType itemAccessState = AccessStateType.Denied;            
            if (CheckUserPermission(PermissionConstants.PermissionName.ManageReportServerProperties))
            {
                itemAccessState = AccessStateType.Allowed;
            }
            else
            {
                BORoleAccess roleAccess = this.RoleAccessCollection[NcaRoleCode, itemPath];
                if (roleAccess == null)
                {
                    // No explicit RoleAccess was found for this CatalogItem, recurse up the 
                    // ancestor folder chain to see if RoleAccess was specified at a higher level
                    if (!itemPath.Equals("/"))
                    {
                        // We reached the root and didn't find a RoleAccess entry, access is denied.
                        int lastSlashIndex = itemPath.LastIndexOf("/");
                        string parentPath = string.Empty;

                        if (lastSlashIndex == 0)
                        {
                            // Special Case: The item's parent is root
                            parentPath = "/";
                        }
                        else
                        {
                            // Standard Case: The item has a parent folder
                            parentPath = itemPath.Substring(0, lastSlashIndex);
                        }

                        itemAccessState = CheckItemAccess(parentPath);
                    }
                }
                else
                {
                    itemAccessState = roleAccess.AccessState;
                }

                BOUserAccess userAccess = this.UserAccessCollection[Username, itemPath];
                if (userAccess != null)
                {
                    //User level overrides Role security
                    itemAccessState = userAccess.AccessState;
                }
            }
            return itemAccessState;
        }

        #endregion

        #region Private Helper Methods
        private void checkClientCacheExpired()
        {
            if ((_reportingServiceClient != null) &&
                 (_reportingServiceClient.AuthCookie != null))
            {
                // Check to see if the cached client connection has expired
                if (_reportingServiceClient.AuthCookie.Expired)
                {
                    _reportingServiceClient.Dispose();
                    _reportingServiceClient = null;

                    _configurationCollection.Clear();
                    _configurationCollection = null;
                }
            }
        }

        /// <summary>
        /// Make sure the UserEntity information in the Application database matches the 
        /// information used to Authenticate against the security system.
        /// </summary>
        private void synchronizeUserToDatabase()
        {
            BOUserEntityCollection users = new BOUserEntityCollection();
            users.Fill();

            BOUserEntity loggedOnUser = users.FindUserEntity(Username, Domain);
            if (loggedOnUser == null)
            {
                loggedOnUser = new BOUserEntity();
            }

            loggedOnUser.Domain = Domain;
            loggedOnUser.UserName = Username;
            loggedOnUser.Password = Utility.EncryptValue(Password);
            loggedOnUser.NCAUserId = _ncaUserId;
            loggedOnUser.NCAExpiration = _ncaExpiration;

            loggedOnUser.Save(AuthenticatedUserSignature);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_reportingServiceClient != null)
                {
                    _reportingServiceClient.Dispose();
                    _reportingServiceClient = null;
                }

                if (_reportingServiceNonHttpClient != null)
                {
                    _reportingServiceNonHttpClient.Dispose();
                    _reportingServiceNonHttpClient = null;
                }

                if (_executionServiceClient != null)
                {
                    _executionServiceClient.Dispose();
                    _executionServiceClient = null;
                }

                if (_reportingTargetServiceClient != null)
                {
                    _reportingTargetServiceClient.Dispose();
                    _reportingTargetServiceClient = null;
                }

                if (_reportingTargetServiceNonHttpClient != null)
                {
                    _reportingTargetServiceNonHttpClient.Dispose();
                    _reportingTargetServiceNonHttpClient = null;
                }

                if (_reportingSourceServiceClient != null)
                {
                    _reportingSourceServiceClient.Dispose();
                    _reportingSourceServiceClient = null;
                }

                if (_reportingSourceServiceNonHttpClient != null)
                {
                    _reportingSourceServiceNonHttpClient.Dispose();
                    _reportingSourceServiceNonHttpClient = null;
                }
            }
        }
        #endregion
    }
}
