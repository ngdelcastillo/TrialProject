using System;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOConfiguredRole : ConfiguredRoleMessage
    {
        #region Constructors

        public BOConfiguredRole()
            : base()
        {
        }

        /// <summary>
        /// This constructor is used by Collections, it is not intended to be publicly consumable
        /// </summary>
        /// <param name="dr">The DataReader used to fill the underlying Message type</param>
        internal BOConfiguredRole(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        {
        }
        #endregion

        #region Methods
        public void Delete()
        {
            base.Delete(this.ConfiguredRoleId);
        }

        public void Save(string authenticatedUser)
        {
            if (this.ConfiguredRoleId == Guid.Empty)
            {
                this.Insert(RoleName, authenticatedUser);
            }
            else
            {
                this.Update(ConfiguredRoleId, RoleName, authenticatedUser);
            }
        }
        #endregion
    }
}
