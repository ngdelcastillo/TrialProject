using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Text;

using Navitaire.Ncl;
using Navitaire.Ncl.Security;
using Navitaire.Ncs.Security.Contract;
using Navitaire.Ncs.Security.Contract.Request;
using Navitaire.Ncs.Security.Contract.Response;
using Navitaire.Reporting.Security.Exceptions;
namespace Navitaire.Reporting.Security
{
    public class AuthenticationManager
    {
        public static AuthenticationInformation Authenticate(string username, string password, string domain)
        {
            AuthenticationInformation authenticationInformation = new AuthenticationInformation();
            string environment = ConfigurationManager.AppSettings["EnvironmentName"];
            ISecurityService securityService = null;
            try
            {
                string securityServiceId = ConfigurationManager.AppSettings["SecurityServiceID"];
                securityService = ComponentManager.GetComponent<ISecurityService>(securityServiceId, environment);
                if (securityService == null)
                {
                    throw new LogonException("Cannot get a reference to ISecurityService from ComponentManager, make sure the ComponentHost is configured correctly and running.");
                }
            }
            catch (Exception ex)
            {
                throw new LogonException(string.Format("Unable to connect to the security service for the '{0}' environment", environment), ex);
            }

            AuthenticationResponse authenticationResponse = createAuthenticationRequest(securityService, username, password, domain);
            authenticationInformation.ActiveRoleCode = authenticationResponse.AuthenticationContext.Principal.RoleCode;
            authenticationInformation.UserAccountExpirationUtc = authenticationResponse.AuthenticationContext.ExpirationUtc;

            Identity ncaIdentity = authenticationResponse.AuthenticationContext.Principal.Identity as Identity;
            if (ncaIdentity != null)
            {
                authenticationInformation.UserId = ncaIdentity.UserId;
            }

            bool changepassword = authenticationResponse.ChangeCredentialsRequired;
            if (changepassword)
            {
                throw new ResetPasswordException();
            }

            authenticationInformation.InitializeSecurityRoleCollection(securityService.GetPrincipalRoleNames(authenticationResponse.AuthenticationContext));
            AuthenticationContextManager.CurrentContext = authenticationResponse.AuthenticationContext;

            List<AuthorizationRequest> authorizationRequests = createReportingAuthorizationRequestList(); //Create a list of permissions we want
            List<AuthorizationResponse> authorizationResponses = securityService.EvaluateAuthorizationRules(AuthenticationContextManager.CurrentContext, authorizationRequests); //Get the state of the permissions from security service
            authenticationInformation.InitializePermissionSet(authorizationResponses); //add permissions in the user information

            return authenticationInformation;
        }

        private static AuthenticationResponse createAuthenticationRequest(ISecurityService securityService, string username, string password, string domain)
        {
            UserNamePasswordCredentials credentials = createCredentials(username, password, domain);
            AuthenticationRequest authenticationRequest = new AuthenticationRequest(credentials);
            return securityService.Authenticate(authenticationRequest);
        }

        private static UserNamePasswordCredentials createCredentials(string username, string password, string domain)
        {
            string clientCode = ConfigurationManager.AppSettings["ClientCode"];
            return new UserNamePasswordCredentials(username, password, domain, clientCode);
        }

        private static AuthorizationRequest createAuthorizationRequest(string securityNameSpace, string action)
        {
            AuthorizationRequest authorizationRequest = new AuthorizationRequest();
            authorizationRequest.Namespace = securityNameSpace;
            authorizationRequest.Action = action;
            return authorizationRequest;
        }

        private static List<AuthorizationRequest> createReportingAuthorizationRequestList()
        {
            List<AuthorizationRequest> authorizationRequests = new List<AuthorizationRequest>();

            //Use System.Reflection to automatically update the list based on the fields in the PermissionConstants.PermissionName class
            Type permissionNameClassType = typeof(PermissionConstants.PermissionName);
            FieldInfo[] permissionNameClassFields = permissionNameClassType.GetFields();
            foreach (FieldInfo permissionNameClassField in permissionNameClassFields)
            {
                if (permissionNameClassField.IsStatic && permissionNameClassField.IsPublic && permissionNameClassField.IsLiteral)
                {
                    string fieldValue = permissionNameClassField.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        authorizationRequests.Add(createAuthorizationRequest(fieldValue, "Allow"));
                    }
                }
            }

            return authorizationRequests;
        }
    }
}
