using System;
using System.Collections.Generic;
using System.Configuration;

using Navitaire.Ncl;
using Navitaire.Ncl.Security;
using Navitaire.Ncl.Security.Cryptography;

using Navitaire.Ncs.Security.Contract;
using Navitaire.Ncs.Security.Contract.Request;
using Navitaire.Ncs.Security.Contract.Response;


namespace Navitaire.Reporting.Security
{
    public class SecurityManager
    {
        #region Methods
        public static string DecryptValue(string encryptedValue)
        {
            return FileCrypto.Decrypt(encryptedValue);
        }

        public static string EncryptValue(string decryptedValue)
        {
            return FileCrypto.Encrypt(decryptedValue);
        }

        public static PermissionCollection GetEffectivePermissions(
            string userName,
            string password,
            string domain)
        {
            PermissionCollection effectivePermissions = null;

            try
            {
                AuthenticationInformation authInfo = AuthenticationManager.Authenticate(userName, password, domain);
                effectivePermissions = new PermissionCollection(authInfo.PermissionCollection);
            }
            catch
            {
            }

            if (effectivePermissions == null)
            {
                effectivePermissions = new PermissionCollection();
            }

            return effectivePermissions;
        }
        #endregion
    }
}
