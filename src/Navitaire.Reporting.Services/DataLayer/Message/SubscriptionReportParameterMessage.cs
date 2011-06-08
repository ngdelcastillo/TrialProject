using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    /// <summary>
    /// SubscriptionReportParameterMessage controls communication to the database and acts as a base class for business layer objects
    /// </summary>
    /// <remarks>
    /// NOTICE: The code in this file is auto-generated.  Any changes made to this file will be
    ///         lost the next time it is generated.  To add custom code onsider a Partial Class,
    ///         or add the custom code to a class that is derived from this base message type.
    /// </remarks>
    public class SubscriptionReportParameterMessage : BaseMessage
    {
        #region Declarations

        private string _connString;
        private string _targetConnString;
        private int _commandTimeout = 30;
        // Primary Key Field
        private System.Guid _SubscriptionReportParameterId = Guid.Empty;
        private System.Guid _SubscriptionId;
        private System.String _ParameterName;
        private System.Int32 _ParameterType;
        private System.String _ParameterValue;
        private System.DateTime _Created = SqlDateTime.MinValue.Value;
        private System.String _CreatedBy;
        private System.DateTime _Modified = SqlDateTime.MinValue.Value;
        private System.String _ModifiedBy;
        private System.Boolean _ParameterUseDefault;


        #endregion Declarations

        #region Properties

        public string ConnectionString
        {
            get { return _connString; }
            set { _connString = value; }
        }
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }
        /// <summary>
        /// Primary Key Property for SubscriptionReportParameter
        /// </summary>
        public System.Guid SubscriptionReportParameterId
        {
            get
            {
                return _SubscriptionReportParameterId;
            }
        }

        public System.Boolean ParameterUseDefault
        {
            get { return _ParameterUseDefault; }
            set { _ParameterUseDefault = value; }
        }

        public System.Guid SubscriptionId
        {
            get
            {
                return _SubscriptionId;
            }
            set
            {
                _SubscriptionId = value;
            }
        }

        public System.String ParameterName
        {
            get
            {
                return _ParameterName;
            }
            set
            {
                _ParameterName = value;
            }
        }

        public System.Int32 ParameterType
        {
            get
            {
                return _ParameterType;
            }
            set
            {
                _ParameterType = value;
            }
        }

        public System.String ParameterValue
        {
            get
            {
                return _ParameterValue;
            }
            set
            {
                _ParameterValue = value;
            }
        }

        public System.DateTime Created
        {
            get
            {
                return _Created;
            }
            set
            {
                _Created = value;
            }
        }

        public System.String CreatedBy
        {
            get
            {
                return _CreatedBy;
            }
            set
            {
                _CreatedBy = value;
            }
        }

        public System.DateTime Modified
        {
            get
            {
                return _Modified;
            }
            set
            {
                _Modified = value;
            }
        }

        public System.String ModifiedBy
        {
            get
            {
                return _ModifiedBy;
            }
            set
            {
                _ModifiedBy = value;
            }
        }

        #endregion Properties

        #region Constructors

        public SubscriptionReportParameterMessage()
        {
            try
            {
                _connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
                _targetConnString = ConfigurationManager.ConnectionStrings["NAVITAIRE-TARGETAPP"].ConnectionString;
            }
            catch { }
        }

        public SubscriptionReportParameterMessage(string connectionString)
        {
            _connString = connectionString;
        }
        public SubscriptionReportParameterMessage(SqlDataReader dr)
            : this()
        {
            LoadFromReader(dr);
        }

        #endregion Constructors

        #region Data Layer Methods

        protected virtual void Delete(Guid subscriptionReportParameterId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Delete(conn, trans, subscriptionReportParameterId);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid subscriptionReportParameterId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteSubscriptionReportParameter", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@subscriptionReportParameterId", subscriptionReportParameterId);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(Guid subscriptionId, string parameterName, int parameterType, string parameterValue, string createdBy, bool useDefault)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Insert(conn, trans, subscriptionId, parameterName, parameterType, parameterValue, createdBy, useDefault);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void InsertTarget(Guid subscriptionId, string parameterName, int parameterType, string parameterValue, string createdBy, bool useDefault)
        {
            using (SqlConnection conn = new SqlConnection(_targetConnString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Insert(conn, trans, subscriptionId, parameterName, parameterType, parameterValue, createdBy, useDefault);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, Guid subscriptionId, string parameterName, int parameterType, string parameterValue, string createdBy, bool useDefault)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertSubscriptionReportParameter", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@subscriptionId", subscriptionId);
                cmd.Parameters.AddWithValue("@parameterName", parameterName);
                cmd.Parameters.AddWithValue("@parameterType", parameterType);
                cmd.Parameters.AddWithValue("@parameterValue", parameterValue);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);
                cmd.Parameters.AddWithValue("@useDefault", useDefault);
                this._SubscriptionReportParameterId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid subscriptionReportParameterId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, subscriptionReportParameterId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid subscriptionReportParameterId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectSubscriptionReportParameter", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@subscriptionReportParameterId", subscriptionReportParameterId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    try
                    {
                        while (dr.Read())
                        {
                            LoadFromReader(dr);
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
            }
        }

        protected virtual void Update(Guid subscriptionReportParameterId, Guid subscriptionId, string parameterName, int parameterType, string parameterValue, string modifiedBy, bool useDefault)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Update(conn, trans, subscriptionReportParameterId, subscriptionId, parameterName, parameterType, parameterValue, modifiedBy, useDefault);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void UpdateTarget(Guid subscriptionReportParameterId, Guid subscriptionId, string parameterName, int parameterType, string parameterValue, string modifiedBy, bool useDefault)
        {
            using (SqlConnection conn = new SqlConnection(_targetConnString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Update(conn, trans, subscriptionReportParameterId, subscriptionId, parameterName, parameterType, parameterValue, modifiedBy, useDefault);
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid subscriptionReportParameterId, Guid subscriptionId, string parameterName, int parameterType, string parameterValue, string modifiedBy, bool useDefault)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateSubscriptionReportParameter", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@subscriptionReportParameterId", subscriptionReportParameterId);
                cmd.Parameters.AddWithValue("@subscriptionId", subscriptionId);
                cmd.Parameters.AddWithValue("@parameterName", parameterName);
                cmd.Parameters.AddWithValue("@parameterType", parameterType);
                cmd.Parameters.AddWithValue("@parameterValue", parameterValue);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);
                cmd.Parameters.AddWithValue("@UseDefault", useDefault);

                cmd.ExecuteScalar();
            }
        }

        #endregion Data Layer Methods

        #region Private Helper Methods

        protected virtual void LoadFromReader(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                _SubscriptionReportParameterId = (System.Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _SubscriptionId = (System.Guid)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _ParameterName = (System.String)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _ParameterType = (System.Int32)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _ParameterValue = (System.String)dr[4];
            }

            if (!dr.IsDBNull(5))
            {
                _Created = (System.DateTime)dr[5];
            }

            if (!dr.IsDBNull(6))
            {
                _CreatedBy = (System.String)dr[6];
            }

            if (!dr.IsDBNull(7))
            {
                _Modified = (System.DateTime)dr[7];
            }

            if (!dr.IsDBNull(8))
            {
                _ModifiedBy = (System.String)dr[8];

            }

            if (!dr.IsDBNull(9))
            {
                _ParameterUseDefault = (System.Boolean)dr[9];
            }
        }

        #endregion Private Helper Methods

    }
}
