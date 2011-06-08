using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public class RoleAccessMessage : BaseMessage
    {
        #region Variables
        private Guid _roleAccessId = Guid.Empty;
        private String _roleCode;
        private String _itemPath;
        private Int16 _accessState;
        private DateTime _created = SqlDateTime.MinValue.Value;
        private String _createdBy;
        private DateTime _modified = SqlDateTime.MinValue.Value;
        private String _modifiedBy;
        #endregion

        #region Properties
        public Guid RoleAccessId
        {
            get
            {
                return _roleAccessId;
            }
        }

        public String RoleCode
        {
            get
            {
                return _roleCode;
            }
            set
            {
                _roleCode = value;
            }
        }

        public String ItemPath
        {
            get
            {
                return _itemPath;
            }
            set
            {
                _itemPath = value;
            }
        }

        public Int16 AccessState
        {
            get
            {
                return _accessState;
            }
            set
            {
                _accessState = value;
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

        public RoleAccessMessage()
        {
            try
            {
                Connection = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }

        public RoleAccessMessage(string connectionString)
        {
            Connection = connectionString;
        }
        public RoleAccessMessage(SqlDataReader dr)
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
                _roleAccessId = (System.Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _roleCode = (System.String)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _itemPath = (System.String)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _accessState = (System.Int16)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _created = (System.DateTime)dr[4];
            }

            if (!dr.IsDBNull(5))
            {
                _createdBy = (System.String)dr[5];
            }

            if (!dr.IsDBNull(6))
            {
                _modified = (System.DateTime)dr[6];
            }

            if (!dr.IsDBNull(7))
            {
                _modifiedBy = (System.String)dr[7];
            }

        }

        #endregion Private Helper Methods

        #region Data Layer Methods

        protected virtual void Delete(Guid roleAccessId)
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
                            Delete(conn, trans, roleAccessId);
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
        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid roleAccessId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteRoleAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@roleAccessId", roleAccessId);

                cmd.ExecuteScalar();
            }
        }

        protected virtual void Insert(string roleCode, string itemPath, short accessState, string createdBy)
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
                            Insert(conn, trans, roleCode, itemPath, accessState, createdBy);
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
        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string roleCode, string itemPath, short accessState, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertRoleAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@roleCode", roleCode);
                cmd.Parameters.AddWithValue("@itemPath", itemPath);
                cmd.Parameters.AddWithValue("@accessState", accessState);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                _roleAccessId = (System.Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid roleAccessId)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                try
                {
                    conn.Open();
                    Select(conn, null, roleAccessId);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid roleAccessId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectRoleAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.Parameters.AddWithValue("@roleAccessId", roleAccessId);

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

        protected virtual void Update(Guid roleAccessId, string roleCode, string itemPath, short accessState, string modifiedBy)
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
                            Update(conn, trans, roleAccessId, roleCode, itemPath, accessState, modifiedBy);
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
        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid roleAccessId, string roleCode, string itemPath, short accessState, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateRoleAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@roleAccessId", roleAccessId);
                cmd.Parameters.AddWithValue("@roleCode", roleCode);
                cmd.Parameters.AddWithValue("@itemPath", itemPath);
                cmd.Parameters.AddWithValue("@accessState", accessState);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                cmd.ExecuteScalar();
            }
        }

        #endregion Data Layer Methods
    }
}
