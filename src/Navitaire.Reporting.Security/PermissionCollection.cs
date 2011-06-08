using System;
using System.Collections.ObjectModel;

namespace Navitaire.Reporting.Security
{
    public class PermissionCollection : Collection<Permission>
    {
        #region Construtors
        public PermissionCollection()
        { }
        public PermissionCollection(PermissionCollection permissions)
        {
            if (permissions != null)
            {
                foreach (Permission permission in permissions)
                {
                    this.Add(new Permission(permission));
                }
            }
        }
        #endregion

        #region Property
        public Permission this[string securityNameSpace]
        {
            get
            {
                foreach (Permission permission in this)
                {
                    if(permission.SecurityNameSpace.Equals(securityNameSpace, StringComparison.OrdinalIgnoreCase))
                    {
                        return permission;
                    }
                }
                return null;
            }
        }
        #endregion
    }
}
