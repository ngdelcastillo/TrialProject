using System;
using Navitaire.Reporting.Services.DataLayer.CollectionGen;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOUserEntityCollection : UserEntityCollectionGen
    {
        #region Properties
        public BOUserEntity this[Guid userId]
        {
            get
            {
                foreach (BOUserEntity userEntity in this)
                {
                    if (userEntity.UserId.Equals(userId))
                    {
                        return userEntity;
                    }
                }

                return null;
            }
        }

        public BOUserEntity this[string userName, string domain]
        {
            get
            {
                return FindUserEntity(userName, domain);
            }
        }
        #endregion

        #region Methods
        public void Fill()
        {
            base.SelectUserEntityList();
        }

        public BOUserEntity FindUserEntity(string userName, string domain)
        {
            BOUserEntity foundUser = null;

            foreach (BOUserEntity currentUser in this)
            {
                if ((currentUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)) &&
                    (currentUser.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase)))
                {
                    foundUser = currentUser;
                    break;
                }
            }

            return foundUser;
        }
        #endregion
    }
}
