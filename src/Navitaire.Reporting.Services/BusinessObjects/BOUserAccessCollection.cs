using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOUserAccessCollection : UserAccessCollectionGen
    {
        #region Properties
        public BOUserAccess this[Guid userAccessID]
        {
            get
            {
                foreach (BOUserAccess userAccess in this)
                {
                    if (userAccess.UserAccessID.Equals(userAccessID))
                    {
                        return userAccess;
                    }
                }
                return null;
            }
        }

        public BOUserAccess this[string userCode, string itemPath]
        {
            get
            {
                foreach (BOUserAccess userAccess in this)
                {
                    if ((userAccess.UserCode.Equals(userCode, StringComparison.OrdinalIgnoreCase)) &&
                        (userAccess.ItemPath.Equals(itemPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        return userAccess;
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
            base.SelectUserAccessList();
        }

        public BOUserAccessCollection FilterForItemPath(string itemPath)
        {
            BOUserAccessCollection filteredUserCollection = new BOUserAccessCollection();
            foreach (BOUserAccess userAccess in this)
            {
                if (userAccess.ItemPath.Equals(itemPath, StringComparison.OrdinalIgnoreCase))
                {
                    filteredUserCollection.Add(userAccess);
                }
            }
            return filteredUserCollection;
        }

        public bool Remove(Guid userAccessID)
        {
            BOUserAccess userAccess = this[userAccessID];

            if (userAccessID != null)
            {
                return this.Remove(userAccess);
            }
            return true;
        }
        #endregion
    }
}
