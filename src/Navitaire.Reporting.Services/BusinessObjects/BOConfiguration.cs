using System;
using System.Data.SqlClient;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    public class BOConfiguration : ConfigurationMessage
    {
        #region Constructors

        public BOConfiguration()
            : base()
        {
        }

        /// <summary>
        /// This constructor is used by Collections, it is not intended to be publicly consumable
        /// </summary>
        /// <param name="dr">The DataReader used to fill the underlying Message type</param>
        internal BOConfiguration(SqlDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructors

        public void Delete()
        {
            base.Delete(this.ConfigurationId);
        }

        public void Save(string authenticatedUser)
        {
            if (this.ConfigurationId == Guid.Empty)
            {
                base.Insert(this.PropertyName, this.PropertyDescription, this.PropertyValue, this.Behavior, authenticatedUser);
            }
            else
            {
                base.Update(this.ConfigurationId, this.PropertyName, this.PropertyDescription, this.PropertyValue, this.Behavior, authenticatedUser);
            }
        }
    }
}
