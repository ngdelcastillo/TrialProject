using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public class ConfiguredDataSourceMessage : BaseMessage
    {
        #region Variables
        private Guid _configuredDataSourceId = Guid.Empty;
        private String _catalogPath;
        private String _connectionStringValue;
        private String _connectionStringType;
        private String _credentials;
        private DateTime _created = SqlDateTime.MinValue.Value;
        private String _createdBy;
        private DateTime _modified = SqlDateTime.MinValue.Value;
        private String _modifiedBy;
        #endregion

        #region Properties
        public System.Guid ConfiguredDataSourceId
        {
            get
            {
                return _configuredDataSourceId;
            }
        }

        public String CatalogPath
        {
            get
            {
                return _catalogPath;
            }
            set
            {
                _catalogPath = value;
            }
        }

        public String ConnectionStringValue
        {
            get
            {
                return _connectionStringValue;
            }
            set
            {
                _connectionStringValue = value;
            }
        }

        public String ConnectionStringType
        {
            get
            {
                return _connectionStringType;
            }
            set
            {
                _connectionStringType = value;
            }
        }

        public String Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
            }
        }

        public DateTime Created
        {
            get
            {
                return _created;
            }
            set
            {
                _created = value;
            }
        }

        public String CreatedBy
        {
            get
            {
                return _createdBy;
            }
            set
            {
                _createdBy = value;
            }
        }

        public DateTime Modified
        {
            get
            {
                return _modified;
            }
            set
            {
                _modified = value;
            }
        }

        public String ModifiedBy
        {
            get
            {
                return _modifiedBy;
            }
            set
            {
                _modifiedBy = value;
            }
        }
        #endregion

        #region Constructors

        public ConfiguredDataSourceMessage()
        {
            try
            {
                Connection = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }

        public ConfiguredDataSourceMessage(string connectionString)
        {
            Connection = connectionString;
        }
        public ConfiguredDataSourceMessage(SqlDataReader dr)
            : this()
        {
            LoadFromReader(dr);
        }

        #endregion Constructors

        #region Private Helper Methods

        protected virtual void LoadFromReader(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                _configuredDataSourceId = (System.Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _catalogPath = (System.String)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _connectionStringValue = (System.String)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _connectionStringType = (System.String)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _credentials = (System.String)dr[4];
            }

            if (!dr.IsDBNull(5))
            {
                _created = (System.DateTime)dr[5];
            }

            if (!dr.IsDBNull(6))
            {
                _createdBy = (System.String)dr[6];
            }

            if (!dr.IsDBNull(7))
            {
                _modified = (System.DateTime)dr[7];
            }

            if (!dr.IsDBNull(8))
            {
                _modifiedBy = (System.String)dr[8];
            }

        }

        #endregion Private Helper Methods

        #region Data Layer Methods

        protected virtual void Delete(Guid configuredDataSourceId)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Delete(conn, trans, configuredDataSourceId);
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
        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid configuredDataSourceId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteConfiguredDataSource", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@configuredDataSourceId", configuredDataSourceId);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(string catalogPath, string connectionStringValue, string connectionStringType, string credentials, string createdBy)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Insert(conn, trans, catalogPath, connectionStringValue, connectionStringType, credentials, createdBy);
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
        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string catalogPath, string connectionStringValue, string connectionStringType, string credentials, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertConfiguredDataSource", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@catalogPath", catalogPath);
                cmd.Parameters.AddWithValue("@connectionStringValue", connectionStringValue);
                cmd.Parameters.AddWithValue("@connectionStringType", connectionStringType);
                cmd.Parameters.AddWithValue("@credentials", credentials);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                _configuredDataSourceId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid configuredDataSourceId)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, configuredDataSourceId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid configuredDataSourceId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectConfiguredDataSource", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@configuredDataSourceId", configuredDataSourceId);

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

        protected virtual void Update(Guid configuredDataSourceId, string catalogPath, string connectionStringValue, string connectionStringType, string credentials, string modifiedBy)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Update(conn, trans, configuredDataSourceId, catalogPath, connectionStringValue, connectionStringType, credentials, modifiedBy);
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
        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid configuredDataSourceId, string catalogPath, string connectionStringValue, string connectionStringType, string credentials, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateConfiguredDataSource", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@configuredDataSourceId", configuredDataSourceId);
                cmd.Parameters.AddWithValue("@catalogPath", catalogPath);
                cmd.Parameters.AddWithValue("@connectionStringValue", connectionStringValue);
                cmd.Parameters.AddWithValue("@connectionStringType", connectionStringType);
                cmd.Parameters.AddWithValue("@credentials", credentials);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                cmd.ExecuteScalar();
            }
        }

        #endregion Data Layer Methods
    }
}
