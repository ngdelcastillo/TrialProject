using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOConfiguredRoleCollection : ConfiguredRoleCollectionGen
    {
        #region Properties
        public BOConfiguredRole this[Guid searchBOConfiguredRoleId]
        {
            get
            {
                foreach (BOConfiguredRole searchBOConfiguredRole in this)
                {
                    if (searchBOConfiguredRole.ConfiguredRoleId.Equals(searchBOConfiguredRoleId))
                    {
                        return searchBOConfiguredRole;
                    }
                }
                return null;
            }
        }

        public BOConfiguredRole this[string roleName]
        {
            get
            {
                foreach (BOConfiguredRole searchBOConfiguredRole in this)
                {
                    if (searchBOConfiguredRole.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return searchBOConfiguredRole;
                    }
                }

                return null;
            }
        }
        #endregion

        #region Methods
        public void Fill()
        {
            this.Clear();

            base.SelectConfiguredRoleList();
        }

        public bool Remove(Guid configuredRoleId)
        {
            BOConfiguredRole theConfiguredRole = this[configuredRoleId];
            if (theConfiguredRole != null)
            {
                return this.Remove(theConfiguredRole);
            }

            return true;
        }
        #endregion
    }
}
