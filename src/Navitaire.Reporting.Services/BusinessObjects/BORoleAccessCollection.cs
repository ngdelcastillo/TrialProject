using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BORoleAccessCollection : RoleAccessCollectionGen
    {
        #region Properties
        public BORoleAccess this[Guid roleAccessId]
        {
            get
            {
                foreach (BORoleAccess roleAccess in this)
                {
                    if (roleAccess.RoleAccessId.Equals(roleAccessId))
                    {
                        return roleAccess;
                    }
                }

                return null;
            }
        }

        public BORoleAccess this[string roleCode, string itemPath]
        {
            get
            {
                foreach (BORoleAccess roleAccess in this)
                {
                    if ((roleAccess.ItemPath.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
                        && (roleAccess.RoleCode.Equals(roleCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        return roleAccess;
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

            base.SelectRoleAccessList();
        }

        public BORoleAccessCollection FilterForItemPath(string itemPath)
        {
            BORoleAccessCollection filteredCollection = new BORoleAccessCollection();

            foreach (BORoleAccess roleAccess in this)
            {
                if (roleAccess.ItemPath.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
                {
                    filteredCollection.Add(roleAccess);
                }
            }

            return filteredCollection;
        }

        public bool Remove(Guid roleAccessId)
        {
            BORoleAccess theRoleAccess = this[roleAccessId];
            if (theRoleAccess != null)
            {
                return this.Remove(theRoleAccess);
            }

            return true;
        }
        #endregion
    }
}
