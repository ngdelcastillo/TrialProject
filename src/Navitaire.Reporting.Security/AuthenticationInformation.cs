using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Navitaire.Ncs.Security.Contract;
using Navitaire.Ncs.Security.Contract.Response;

namespace Navitaire.Reporting.Security
{
    public class AuthenticationInformation
    {
        #region Variables
        private string _activeRoleCode;
        private PermissionCollection _permissionCollection;
        private StringCollection _securityRoleCollection = new StringCollection();
        private DateTime _userAccountExpirationUtc;
        private long _userId;
        #endregion

        #region Constructor
        public AuthenticationInformation()
        { }
        #endregion

        #region Properties
        public string ActiveRoleCode
        {
            get
            {
                return _activeRoleCode;
            }
            set
            {
                _activeRoleCode = value;
            }
        }

        public PermissionCollection PermissionCollection
        {
            get
            {
                return _permissionCollection;
            }
        }

        public StringCollection SecurityRoleCollection
        {
            get
            {
                return _securityRoleCollection;
            }
        }

        public DateTime UserAccountExpirationUtc
        {
            get
            {
                return _userAccountExpirationUtc;
            }
            set
            {
                _userAccountExpirationUtc = value;
            }
        }

        public long UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
        #endregion

        #region Methods
        internal void InitializeSecurityRoleCollection(List<string> roleList)
        {
            foreach (string role in roleList)
            {
                _securityRoleCollection.Add(role);
            }
        }

        internal void InitializePermissionSet(List<AuthorizationResponse> responses)
        {
            if (_permissionCollection == null)
            {
                _permissionCollection = new PermissionCollection();
            }
            
            _permissionCollection.Clear();
            foreach (AuthorizationResponse response in responses)
            {
                bool allowed = false;
                if (response.EvaluationResult == AuthorizationEvaluationResult.Allowed)
                {
                    allowed = true;
                }

                _permissionCollection.Add(new Permission(
                    response.Request.Namespace,
                    response.Request.Action,
                    allowed));
            }
        }
        #endregion
    }
}
