using System;
using Navitaire.Reporting.Services;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BORoleAccess : RoleAccessMessage
    {
        #region Constructors
        public BORoleAccess()
            : base()
        { }

        internal BORoleAccess(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        { }
        #endregion

        #region Methods
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

        public void Delete()
        {
            base.Delete(base.RoleAccessId);
        }

        public void Save(string authenticatedUser)
        {
            if (this.RoleAccessId == Guid.Empty)
            {
                base.Insert(
                    base.RoleCode,
                    base.ItemPath,
                    base.AccessState,
                    authenticatedUser);
            }
            else
            {
                base.Update(
                    base.RoleAccessId,
                    base.RoleCode,
                    base.ItemPath,
                    base.AccessState,
                    authenticatedUser);
            }
        }
        #endregion
    }
}
