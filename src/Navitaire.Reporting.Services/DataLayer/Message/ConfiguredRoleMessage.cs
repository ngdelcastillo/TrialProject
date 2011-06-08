using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public class ConfiguredRoleMessage : BaseMessage
    {
        #region Variables
        private Guid _configuredRoleId = Guid.Empty;
        private String _roleName;
        private DateTime _created = SqlDateTime.MinValue.Value;
        private String _createdBy;
        private DateTime _modified = SqlDateTime.MinValue.Value;
        private String _modifiedBy;
        #endregion

        #region Properties
        public Guid ConfiguredRoleId
        {
            get
            {
                return _configuredRoleId;
            }
        }

        public String RoleName
        {
            get
            {
                return _roleName;
            }
            set
            {
                _roleName = value;
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

        public ConfiguredRoleMessage()
        {
            try
            {
                Connection = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }

        public ConfiguredRoleMessage(string connectionString)
        {
            Connection = connectionString;
        }
        public ConfiguredRoleMessage(SqlDataReader dr)
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
                _configuredRoleId = (System.Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _roleName = (System.String)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _created = (System.DateTime)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _createdBy = (System.String)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _modified = (System.DateTime)dr[4];
            }

            if (!dr.IsDBNull(5))
            {
                _modifiedBy = (System.String)dr[5];
            }

        }

        #endregion Private Helper Methods

        #region Methods
        protected virtual void Delete(Guid configuredRoleId)
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
                            Delete(conn, trans, configuredRoleId);
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
        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid configuredRoleId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteConfiguredRole", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@configuredRoleId", configuredRoleId);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(string roleName, string createdBy)
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
                            Insert(conn, trans, roleName, createdBy);
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
        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string roleName, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertConfiguredRole", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@roleName", roleName);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                _configuredRoleId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid configuredRoleId)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, configuredRoleId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid configuredRoleId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectConfiguredRole", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@configuredRoleId", configuredRoleId);

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

        protected virtual void Update(Guid configuredRoleId, string roleName, string modifiedBy)
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
                            Update(conn, trans, configuredRoleId, roleName, modifiedBy);
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
        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid configuredRoleId, string roleName, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateConfiguredRole", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@configuredRoleId", configuredRoleId);
                cmd.Parameters.AddWithValue("@roleName", roleName);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                cmd.ExecuteScalar();
            }
        }
        #endregion
    }
}
