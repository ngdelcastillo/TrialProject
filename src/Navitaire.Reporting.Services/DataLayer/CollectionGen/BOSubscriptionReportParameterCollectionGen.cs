using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Navitaire.Reporting.Services.BusinessObjects;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Navitaire.Reporting.Services.DataLayer.CollectionGen
{
    /// <summary>
    /// BOSubscriptionReportParameterCollectionGen is an auto-generated collection of BOSubscriptionReportParameter.
    /// This class has methods to fill the collection based on information
    /// in the Database layer.
    /// </summary>
    /// <remarks>
    /// NOTICE: The code in this file is auto-generated.  Any changes made to this file will be
    ///         lost the next time it is generated.  To add custom code onsider a Partial Class,
    ///         or add the custom code to a class that is derived from this base message type.
    /// </remarks>
    public class BOSubscriptionReportParameterCollectionGen : Collection<BOSubscriptionReportParameter>
    {
        #region Declarations

        private string _connString;
        private string _targetConnString;
        private int _commandTimeout = 30;

        #endregion Declarations

        #region Properties

        public string ConnectionString
        {
            get
            {
                return _connString;
            }
            set
            {
                _connString = value;
            }
        }
        public int CommandTimeout
        {
            get
            {
                return _commandTimeout;
            }
            set
            {
                _commandTimeout = value;
            }
        }

        #endregion Properties

        #region Constructors

        public BOSubscriptionReportParameterCollectionGen()
        {
            try
            {
                _connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
                _targetConnString = ConfigurationManager.ConnectionStrings["NAVITAIRE-TARGETAPP"].ConnectionString;
            }
            catch
            {
            }
        }

        #endregion Constructors

        #region Data Layer Methods

        protected virtual void SelectSubscriptionReportParameterList()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("app.SelectSubscriptionReportParameterList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = _commandTimeout;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                this.Add(new BOSubscriptionReportParameter(dr));
                            }
                            dr.Close();
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void SelectTargetSubscriptionReportParameterList()
        {
            using (SqlConnection conn = new SqlConnection(_targetConnString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("app.SelectSubscriptionReportParameterList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = _commandTimeout;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                this.Add(new BOSubscriptionReportParameter(dr));
                            }
                            dr.Close();
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void SelectSubscriptionReportParameterListBySubscriptionId(Guid subscriptionId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();

                    SelectSubscriptionReportParameterListBySubscriptionId(conn, null, subscriptionId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void SelectSubscriptionReportParameterListBySubscriptionId(SqlConnection conn, SqlTransaction trans, Guid subscriptionId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectSubscriptionReportParameterListBySubscriptionId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@subscriptionId", subscriptionId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        this.Add(new BOSubscriptionReportParameter(dr));
                    }
                    dr.Close();
                }
            }
        }

        #endregion Data Layer Methods
    }
}