using System;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOUserAccess : UserAccessMessage
    {
        #region Constructor
        public BOUserAccess()
            : base()
        { }

        internal BOUserAccess(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        { }
        #endregion

        #region Properties
        public new AccessStateType AccessState
        {
            get
            {
                return (AccessStateType)base.AccessState;
            }
            set
            {
                base.AccessState = (Int16)value;
            }
        }
        #endregion

        #region Methods
        public void Delete()
        {
            base.Delete(base.UserAccessID);
        }

        public void Save(string authenticatedUser)
        {
            if (this.UserAccessID == Guid.Empty)
            {
                base.Insert(
                    base.UserCode,
                    base.ItemPath,
                    base.AccessState,
                    authenticatedUser);
            }
            else
            {
                base.Update(
                    base.UserAccessID,
                    base.UserCode,
                    base.ItemPath,
                    base.AccessState,
                    authenticatedUser);
            }
        }
        #endregion

    }
}
