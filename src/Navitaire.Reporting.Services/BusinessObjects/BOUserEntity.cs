using System;
using System.Globalization;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOUserEntity : UserEntityMessage
    {
        #region Constructors

        public BOUserEntity()
            : base()
        { }

        /// <summary>
        /// This constructor is used by Collections, it is not intended to be publicly consumable
        /// </summary>
        /// <param name="dr">The DataReader used to fill the underlying Message type</param>
        internal BOUserEntity(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        { }

        #endregion Constructors

        #region Property
        public string UserSignature
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", this.Domain, this.UserName);
            }
        }
        #endregion

        #region Methods
        public void Delete()
        {
            base.Delete(base.UserId);
        }

        public void Save(string authenticatedUser)
        {
            if (this.UserId == Guid.Empty)
            {
                base.Insert(
                    base.Domain,
                    base.UserName,
                    base.Password,
                    base.NCAUserId,
                    base.NCAExpiration,
                    authenticatedUser);
            }
            else
            {
                base.Update(
                    this.UserId,
                    base.Domain,
                    base.UserName,
                    base.Password,
                    base.NCAUserId,
                    base.NCAExpiration,
                    authenticatedUser);
            }
        }
        #endregion
    }
}
