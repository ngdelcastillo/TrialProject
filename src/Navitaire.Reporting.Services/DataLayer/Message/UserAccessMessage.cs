using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Navitaire.Reporting.Services.DataLayer.Message
{
    public class UserAccessMessage : BaseMessage
    {
        #region Variables
        private Guid _userAccessId = Guid.Empty;
        private string _usercode = string.Empty;
        private string _itemPath = string.Empty;
        private Int16 _accessState;
        private DateTime _created;
        private string _createdBy = string.Empty;
        private DateTime _modified;
        private string _modifiedBy = string.Empty;
        #endregion

        #region Properties
        public Guid UserAccessID
        {
            //No set since value is automatically assigned
            get { return _userAccessId; }
        }

        public string UserCode
        {
            get { return _usercode; }
            set { _usercode = value; }
        }

        public string ItemPath
        {
            get { return _itemPath; }
            set { _itemPath = value; }
        }

        public Int16 AccessState
        {
            get { return _accessState; }
            set { _accessState = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public DateTime Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public string ModifiedBy
        {
            get { return _modifiedBy; }
            set { _modifiedBy = value; }
        }
        #endregion

        #region Constructors
        public UserAccessMessage()
        {
            try
            {
                Connection = ConfigurationManager.ConnectionStrings["NAVITAIRE-REPORTING"].ConnectionString;
            }
            catch { }
        }

        public UserAccessMessage(string connectionString)
        {
            //Overload that supplies the connection string
            Connection = connectionString;
        }

        public UserAccessMessage(SqlDataReader dr)
            : this()
        { 
            //Create LoadfromReader method
            LoadfromReader(dr);
        }
        #endregion

        #region Private Helper Layer
        protected virtual void LoadfromReader(SqlDataReader dr)
        {
            if (!dr.IsDBNull(0))
            {
                _userAccessId = (Guid)dr[0];
            }

            if (!dr.IsDBNull(1))
            {
                _usercode = (string)dr[1];
            }

            if (!dr.IsDBNull(2))
            {
                _itemPath = (string)dr[2];
            }

            if (!dr.IsDBNull(3))
            {
                _accessState = (Int16)dr[3];
            }

            if (!dr.IsDBNull(4))
            {
                _created = (DateTime)dr[4];
            }

            if (!dr.IsDBNull(5))
            {
                _createdBy = (string)dr[5];
            }

            if (!dr.IsDBNull(6))
            {
                _modified = (DateTime)dr[6];
            }

            if (!dr.IsDBNull(7))
            {
                _modifiedBy = (string)dr[7];
            }
        }
        #endregion

        #region Methods
        protected virtual void Insert(string userCode, string itemPath, Int16 accessState, string createdBy)
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
                            //Create Insert SqlTransaction
                            Insert(conn, trans, userCode, itemPath, accessState, createdBy);
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

        protected virtual void Insert(SqlConnection conn, SqlTransaction trans, string userCode, string itemPath, Int16 accessState, string createdBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.InsertUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@userCode", userCode);
                cmd.Parameters.AddWithValue("@itemPath", itemPath);
                cmd.Parameters.AddWithValue("@accessState", accessState);
                cmd.Parameters.AddWithValue("@createdBy", createdBy);

                //order by 
                _userAccessId = (Guid)cmd.ExecuteScalar();
            }
        }

        protected virtual void Update(Guid userAccessId, string userCode, string itemPath, Int16 accessState, string modifiedBy)
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
                            //Create Update SqlTransaction
                            Update(conn, trans, userAccessId, userCode, itemPath, accessState, modifiedBy);
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

        protected virtual void Update(SqlConnection conn, SqlTransaction trans, Guid userAccessId, string userCode, string itemPath, Int16 accessState, string modifiedBy)
        {
            using (SqlCommand cmd = new SqlCommand("app.UpdateUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@userAccessId", userAccessId);
                cmd.Parameters.AddWithValue("@userCode", userCode);
                cmd.Parameters.AddWithValue("@itemPath", itemPath);
                cmd.Parameters.AddWithValue("@accessState", accessState);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedBy);

                //order by 
                cmd.ExecuteScalar();
            }
        }

        protected virtual void Delete(Guid userAccessId)
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
                            //Create Update SqlTransaction
                            Delete(conn, trans, userAccessId);
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

        protected virtual void Delete(SqlConnection conn, SqlTransaction trans, Guid userAccessId)
        {
            using (SqlCommand cmd = new SqlCommand("app.DeleteUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@userAccessId", userAccessId);
                cmd.ExecuteScalar();
            }
        }

        protected virtual void Select(Guid userAccessId)
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
                            //Create Select SqlTransaction
                            Select(conn, trans, userAccessId);
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

        protected virtual void Select(SqlConnection conn, SqlTransaction trans, Guid userAccessId)
        {
            using (SqlCommand cmd = new SqlCommand("app.SelectUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = Timeout;
                cmd.Transaction = trans;

                cmd.Parameters.AddWithValue("@userAccessId", userAccessId);
                cmd.ExecuteScalar();
            }
        }
        #endregion
    }
}
