using System;
using System.Data.SqlClient;
using Navitaire.Reporting.Services.DataLayer.Message;

namespace Navitaire.Reporting.Services.BusinessObjects
{
    /// <summary>
    /// BOSubscriptionReportParameter contains custom business logic associated with SubscriptionReportParameterMessage
    /// </summary>
    public class BOSubscriptionReportParameter : SubscriptionReportParameterMessage
    {
        #region Constructors

        public BOSubscriptionReportParameter()
            : base()
        {
        }

        /// <summary>
        /// This constructor is used by Collections, it is not intended to be publicly consumable
        /// </summary>
        /// <param name="dr">The DataReader used to fill the underlying Message type</param>
        internal BOSubscriptionReportParameter(System.Data.SqlClient.SqlDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructors

        public new ReportParameterType ParameterType
        {
            get
            {
                return (ReportParameterType)base.ParameterType;
            }
            set
            {
                base.ParameterType = (int)value;
            }
        }

        public void Delete()
        {
            base.Delete(this.SubscriptionReportParameterId);
        }

        public void Delete(SqlConnection conn, SqlTransaction trans)
        {
            base.Delete(conn, trans, this.SubscriptionReportParameterId);
        }

        public void Save(string authenticatedUser)
        {
            if (this.SubscriptionReportParameterId == Guid.Empty)
            {
                this.Insert(this.SubscriptionId,
                    this.ParameterName,
                    base.ParameterType,
                    this.ParameterValue,
                    authenticatedUser,
                    this.ParameterUseDefault);
            }
            else
            {
                this.Update(this.SubscriptionReportParameterId,
                    this.SubscriptionId,
                    this.ParameterName,
                    base.ParameterType,
                    this.ParameterValue,
                    authenticatedUser,
                    this.ParameterUseDefault);
            }
        }

        public void SaveTarget(string authenticatedUser, bool insert, Guid subscriptionReportParameterId, Guid subscriptionId)
        {
            if (insert)
            {
                this.InsertTarget(subscriptionId,
                    this.ParameterName,
                    base.ParameterType,
                    this.ParameterValue,
                    authenticatedUser,
                    this.ParameterUseDefault);
            }
            else
            {
                this.UpdateTarget(subscriptionReportParameterId,
                    subscriptionId,
                    this.ParameterName,
                    base.ParameterType,
                    this.ParameterValue,
                    authenticatedUser,
                    this.ParameterUseDefault);
            }
        }

        public void Save(SqlConnection conn, SqlTransaction trans, string authenticatedUser)
        {
            if (this.SubscriptionReportParameterId == Guid.Empty)
            {
                this.Insert(conn, trans,
                    this.SubscriptionId,
                    this.ParameterName,
                    base.ParameterType,
                    this.ParameterValue,
                    authenticatedUser,
                    this.ParameterUseDefault);
            }
            else
            {
                this.Update(conn, trans,
                    this.SubscriptionReportParameterId,
                    this.SubscriptionId,
                    this.ParameterName,
                    base.ParameterType,
                    this.ParameterValue,
                    authenticatedUser,
                    this.ParameterUseDefault);
            }
        }
    }
}