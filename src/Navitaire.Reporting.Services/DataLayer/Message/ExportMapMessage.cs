using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    /// <summary>
    /// ExportMapMessage controls communication to the database and acts as a base class for business layer objects
    /// </summary>
    /// <remarks>
    /// NOTICE: The code in this file is auto-generated.  Any changes made to this file will be
    ///         lost the next time it is generated.  To add custom code onsider a Partial Class,
    ///         or add the custom code to a class that is derived from this base message type.
    /// </remarks>
    public class ExportMapMessage : BaseMessage
    {
        #region Declarations

        private string _connString;
        private string _targetConnString;
        private int _commandTimeout = 30;
        // Primary Key Field
        private System.Guid _ExportMapId = Guid.Empty;
        private System.String _ReportPath;
        private System.String _DataSourcePath;
        private System.String _ProcName;
        private System.String _DisplayOptions;
        private System.DateTime _Created = SqlDateTime.MinValue.Value;
        private System.String _CreatedBy;
        private System.DateTime _Modified = SqlDateTime.MinValue.Value;
        private System.String _ModifiedBy;

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
        /// Primary Key Property for ExportMap
        /// </summary>
        public System.Guid ExportMapId
        {
            get
            {
                return _ExportMapId;
            }
        }

        public System.String ReportPath
        {
            get
            {
                return _ReportPath;
            }
            set
            {
                _ReportPath = value;
            }
        }

        public System.String DataSourcePath
        {
            get
            {
                return _DataSourcePath;
            }
            set
            {
                _DataSourcePath = value;
            }
        }

        public System.String ProcName
        {
            get
            {
                return _ProcName;
            }
            set
            {
                _ProcName = value;
            }
        }

        public System.String DisplayOptions
        {
            get
            {
                return _DisplayOptions;
            }
            set
            {
                _DisplayOptions = value;
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

        public ExportMapMessage()
        {
            try
            {
                _connString = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
                _targetConnString = ConfigurationManager.ConnectionStrings["NAVITAIRE-TARGETAPP"].ConnectionString;
            }
            catch { }
        }

        public ExportMapMessage(string connectionString)
        {
            _connString = connectionString;
        }
        public ExportMapMessage(SqlDataReader dr)
            : this()
        {
            LoadFromReader(dr);
        }

        #endregion Constructors

        #region Data Layer Methods

        protected virtual void Delete(Guid exportMapId)
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
                            Delete(conn, trans, exportMapId);
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
        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid exportMapId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteExportMap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@exportMapId", exportMapId);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(string reportPath, string dataSourcePath, string procName, string displayOptions, string createdBy)
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
                            Insert(conn, trans, reportPath, dataSourcePath, procName, displayOptions, createdBy);
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

        protected virtual void InsertTarget(string reportPath, string dataSourcePath, string procName, string displayOptions, string createdBy)
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
                            Insert(conn, trans, reportPath, dataSourcePath, procName, displayOptions, createdBy);
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
        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string reportPath, string dataSourcePath, string procName, string displayOptions, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertExportMap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@reportPath", reportPath);
                cmd.Parameters.AddWithValue("@dataSourcePath", dataSourcePath);
                cmd.Parameters.AddWithValue("@procName", procName);
                cmd.Parameters.AddWithValue("@displayOptions", displayOptions);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                this._ExportMapId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid exportMapId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, exportMapId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid exportMapId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectExportMap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@exportMapId", exportMapId);

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

        protected virtual void Update(Guid exportMapId, string reportPath, string dataSourcePath, string procName, string displayOptions, string modifiedBy)
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
                            Update(conn, trans, exportMapId, reportPath, dataSourcePath, procName, displayOptions, modifiedBy);
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

        protected virtual void UpdateTarget(Guid exportMapId, string reportPath, string dataSourcePath, string procName, string displayOptions, string modifiedBy)
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
                            Update(conn, trans, exportMapId, reportPath, dataSourcePath, procName, displayOptions, modifiedBy);
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
        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid exportMapId, string reportPath, string dataSourcePath, string procName, string displayOptions, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateExportMap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@exportMapId", exportMapId);
                cmd.Parameters.AddWithValue("@reportPath", reportPath);
                cmd.Parameters.AddWithValue("@dataSourcePath", dataSourcePath);
                cmd.Parameters.AddWithValue("@procName", procName);
                cmd.Parameters.AddWithValue("@displayOptions", displayOptions);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                cmd.ExecuteScalar();
            }
        }

        #endregion Data Layer Methods

        #region Private Helper Methods

        protected virtual void LoadFromReader(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                _ExportMapId = (System.Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _ReportPath = (System.String)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _DataSourcePath = (System.String)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _ProcName = (System.String)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _DisplayOptions = (System.String)dr[4];
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

        }

        #endregion Private Helper Methods

    }
}
